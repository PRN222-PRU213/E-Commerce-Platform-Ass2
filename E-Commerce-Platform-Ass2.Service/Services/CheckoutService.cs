using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Service.Helper;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemtRepository _orderItemtRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly TransactionCode _transactionCodeHelper;

        public CheckoutService(ICartRepository cartRepository, ICartItemRepository cartItemRepository,
            IOrderRepository orderRepository, IOrderItemtRepository orderItemtRepository,
            IPaymentRepository paymentRepository, IWalletRepository walletRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            _orderRepository = orderRepository;
            _orderItemtRepository = orderItemtRepository;
            _paymentRepository = paymentRepository;
            _walletRepository = walletRepository;
            _transactionCodeHelper = new TransactionCode();
        }

        public async Task<Order> CreateOrderAsync(Guid userId, string shippingAddress, List<Guid> selectedCartItemIds)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);

            if (cart == null || !cart.CartItems.Any())
            {
                throw new Exception("Cart is empty");
            }

            var selectItems = cart.CartItems
                .Where(ci => selectedCartItemIds.Contains(ci.Id))
                .ToList();

            if (!selectItems.Any())
            {
                throw new Exception("No selected items");
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ShippingAddress = shippingAddress,
                Status = "Paid",
                CreatedAt = DateTime.Now,
                TotalAmount = selectItems.Sum(ci =>
                    ci.ProductVariant.Price * ci.Quantity)
            };

            await _orderRepository.AddAsync(order);

            var orderItems = selectItems.Select(ci => new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductVariantId = ci.ProductVariantId,
                ProductName = ci.ProductVariant.VariantName,
                Price = ci.ProductVariant.Price,
                Quantity = ci.Quantity
            }).ToList();

            await _orderItemtRepository.AddRangeAsync(orderItems);

            await _cartItemRepository.DeleteByIdsAsync(
                selectItems.Select(ci => ci.Id).ToList());

            var remainingItems = cart.CartItems
                .Where(ci => !selectedCartItemIds.Contains(ci.Id))
                .ToList();

            if (!remainingItems.Any())
            {
                await _cartRepository.DeleteAsync(cart);
            }

            return order;
        }

        public async Task<Order> ConfirmPaymentAsync(Guid userId, string shippingAddress, List<Guid> selectedCartItemIds, decimal walletUsed, decimal momoAmount)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);

            if (walletUsed > 0)
            {
                if (wallet.Balance < walletUsed)
                    throw new Exception("Số dư ví không đủ");

                wallet.Balance -= walletUsed;
                wallet.LastChangeAmount = -walletUsed;
                wallet.LastChangeType = "Payment";
                wallet.UpdatedAt = DateTime.Now;

                await _walletRepository.UpdateAsync(wallet);
            }

            var order = await CreateOrderAsync(userId, shippingAddress, selectedCartItemIds);

            // ✅ Sử dụng TransactionCode helper để tạo mã giao dịch duy nhất
            string transactionCode = _transactionCodeHelper.GenerateTransactionCode(order.Id);

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Amount = order.TotalAmount,
                Method = walletUsed > 0 && momoAmount > 0 ? "WALLET + MOMO"
                       : walletUsed > 0 ? "WALLET"
                       : "MOMO",
                Status = "Paid",
                TransactionCode = transactionCode,
                PaidAt = DateTime.UtcNow
            };

            await _paymentRepository.AddAsync(payment);

            return order;
        }
    }
}
