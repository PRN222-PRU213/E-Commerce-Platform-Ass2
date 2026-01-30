using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Models;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class ShopWalletService : IShopWalletService
    {
        private readonly IShopWalletRepository _shopWalletRepository;
        private readonly IShopWalletTransactionRepository _transactionRepository;
        private readonly IOrderRepository _orderRepository;

        public ShopWalletService(
            IShopWalletRepository shopWalletRepository,
            IShopWalletTransactionRepository transactionRepository,
            IOrderRepository orderRepository)
        {
            _shopWalletRepository = shopWalletRepository;
            _transactionRepository = transactionRepository;
            _orderRepository = orderRepository;
        }

        public async Task<ShopWalletDto> GetOrCreateAsync(Guid shopId)
        {
            var wallet = await _shopWalletRepository.GetOrCreateAsync(shopId);
            return new ShopWalletDto
            {
                Id = wallet.Id,
                ShopId = wallet.ShopId,
                Balance = wallet.Balance,
                UpdatedAt = wallet.UpdatedAt
            };
        }

        public async Task<ServiceResult> ReceiveOrderPaymentAsync(Guid shopId, Guid orderId, decimal amount)
        {
            if (amount <= 0)
                return ServiceResult.Failure("Số tiền phải lớn hơn 0");

            var wallet = await _shopWalletRepository.GetOrCreateAsync(shopId);

            // Cộng tiền vào ví
            wallet.Balance += amount;
            await _shopWalletRepository.UpdateAsync(wallet);

            // Ghi log giao dịch
            var transaction = new ShopWalletTransaction
            {
                Id = Guid.NewGuid(),
                ShopWalletId = wallet.Id,
                OrderId = orderId,
                TransactionType = "Sale",
                Amount = amount,
                BalanceAfter = wallet.Balance,
                Description = $"Nhận tiền từ đơn hàng #{orderId.ToString()[..8].ToUpper()}",
                CreatedAt = DateTime.Now
            };
            await _transactionRepository.AddAsync(transaction);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> RefundOrderPaymentAsync(Guid shopId, Guid orderId, decimal amount)
        {
            if (amount <= 0)
                return ServiceResult.Failure("Số tiền hoàn phải lớn hơn 0");

            var wallet = await _shopWalletRepository.GetOrCreateAsync(shopId);

            if (wallet.Balance < amount)
                return ServiceResult.Failure("Số dư ví shop không đủ để hoàn tiền");

            // Trừ tiền từ ví
            wallet.Balance -= amount;
            await _shopWalletRepository.UpdateAsync(wallet);

            // Ghi log giao dịch
            var transaction = new ShopWalletTransaction
            {
                Id = Guid.NewGuid(),
                ShopWalletId = wallet.Id,
                OrderId = orderId,
                TransactionType = "Refund",
                Amount = -amount,
                BalanceAfter = wallet.Balance,
                Description = $"Hoàn tiền đơn hàng #{orderId.ToString()[..8].ToUpper()}",
                CreatedAt = DateTime.Now
            };
            await _transactionRepository.AddAsync(transaction);

            return ServiceResult.Success();
        }

        public async Task<List<ShopWalletTransactionDto>> GetTransactionsAsync(Guid shopId, int limit = 20)
        {
            var wallet = await _shopWalletRepository.GetByShopIdAsync(shopId);
            if (wallet == null)
                return new List<ShopWalletTransactionDto>();

            var transactions = await _transactionRepository.GetByShopWalletIdAsync(wallet.Id, limit);

            return transactions.Select(t => new ShopWalletTransactionDto
            {
                Id = t.Id,
                OrderId = t.OrderId,
                TransactionType = t.TransactionType,
                Amount = t.Amount,
                BalanceAfter = t.BalanceAfter,
                Description = t.Description,
                ReferenceId = t.ReferenceId,
                CreatedAt = t.CreatedAt
            }).ToList();
        }

        /// <summary>
        /// Phân phối tiền đơn hàng cho các shops dựa trên OrderItems
        /// </summary>
        public async Task DistributeOrderPaymentAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null) return;

            // Group items by ShopId và tính subtotal cho mỗi shop
            var shopPayments = order.OrderItems
                .Where(item => item.ProductVariant?.Product?.ShopId != null)
                .GroupBy(item => item.ProductVariant!.Product!.ShopId)
                .Select(g => new
                {
                    ShopId = g.Key,
                    Amount = g.Sum(i => i.Price * i.Quantity)
                })
                .ToList();

            // Cộng tiền vào ví mỗi shop
            foreach (var payment in shopPayments)
            {
                await ReceiveOrderPaymentAsync(payment.ShopId, orderId, payment.Amount);
            }
        }
    }
}
