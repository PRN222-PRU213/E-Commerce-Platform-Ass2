using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class RefundService : IRefundService
    {
        private readonly IRefundRepository _refundRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMomoApi _momoApi;
        private readonly IOrderRepository _orderRepository;
        private readonly IWalletRepository _walletRepository;

        public RefundService(
            IRefundRepository refundRepository,
            IPaymentRepository paymentRepository,
            IMomoApi momoApi,
            IOrderRepository orderRepository,
            IWalletRepository walletRepository)
        {
            _refundRepository = refundRepository;
            _paymentRepository = paymentRepository;
            _momoApi = momoApi;
            _orderRepository = orderRepository;
            _walletRepository = walletRepository;
        }

        public async Task RefundAsync(Guid orderId, decimal amount, string reason)
        {
            // 1. Validate order
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new Exception("Order not found.");

            // 2. Validate payment
            var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
            if (payment == null)
                throw new Exception("Payment not found.");
            if (payment.Status == "Refunded")
                throw new Exception("Payment not refundable");

            var requestId = Guid.NewGuid().ToString();

            // 3. Check for duplicate requestId (extremely rare but guard it)
            var isExisted = await _refundRepository.ExistsRequestIdAsync(requestId);
            if (isExisted)
                throw new Exception("Duplicate refund request");

            // 4. Call MoMo API only when payment was made via MoMo
            var isMomoPayment = payment.Method?.Equals("MoMo", StringComparison.OrdinalIgnoreCase) == true;
            if (isMomoPayment)
            {
                // IMomoApi.RefundAsync(transId, amount, requestId)
                var transId = payment.TransactionCode;
                var momoResult = await _momoApi.RefundAsync(transId, amount, requestId);
                if (momoResult.ResultCode != 0)
                    throw new Exception("MoMo refund failed");
            }
            // For Wallet / COD / other methods → skip MoMo, credit wallet directly below

            // 5. Save refund record
            var refund = new Refund
            {
                Id = Guid.NewGuid(),
                PaymentId = payment.Id,
                RequestId = requestId,
                RefundAmount = amount,
                Reason = reason,
                Status = "Success",
                CreatedAt = DateTime.UtcNow
            };
            await _refundRepository.AddAsync(refund);

            // 6. Mark payment as refunded
            payment.Status = "Refunded";
            await _paymentRepository.UpdateAsync(payment);

            // 7. Cancel the order
            order.Status = "Cancelled";
            await _orderRepository.UpdateAsync(order);

            // 8. Credit the user's wallet
            var wallet = await _walletRepository.GetByUserIdAsync(order.UserId);
            if (wallet == null)
            {
                wallet = new Wallet
                {
                    WalletId = Guid.NewGuid(),
                    UserId = order.UserId,
                    Balance = 0,
                    UpdatedAt = DateTime.UtcNow
                };
                await _walletRepository.AddAsync(wallet);
            }

            wallet.Balance += amount;
            wallet.LastChangeAmount = amount;
            wallet.LastChangeType = "Refund";
            wallet.UpdatedAt = DateTime.UtcNow;
            await _walletRepository.UpdateAsync(wallet);
        }

        public async Task<bool> IsRefundedAsync(Guid orderId)
        {
            var payment = await _paymentRepository.GetByOrderIdAsync(orderId);
            return payment != null && payment.Status == "Refunded";
        }
    }
}
