using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Helper;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class PreOrderService : IPreOrderService
    {
        private const string OrderTypePreOrder = "PreOrder";
        private const string PreOrderStatusDepositPending = "DEPOSIT_PENDING";
        private const string PreOrderStatusDepositPaid = "DEPOSIT_PAID";
        private const string PreOrderStatusReadyForFinalPayment = "READY_FOR_FINAL_PAYMENT";
        private const string PreOrderStatusCompleted = "COMPLETED";
        private const string PreOrderStatusExpired = "EXPIRED";

        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemtRepository _orderItemRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IShopRepository _shopRepository;
        private readonly IPreOrderDetailRepository _preOrderDetailRepository;
        private readonly IPreOrderPolicyItemRepository _preOrderPolicyItemRepository;
        private readonly TransactionCode _transactionCode;

        public PreOrderService(
            IProductVariantRepository productVariantRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IOrderItemtRepository orderItemRepository,
            IPaymentRepository paymentRepository,
            IWalletRepository walletRepository,
            IShopRepository shopRepository,
            IPreOrderDetailRepository preOrderDetailRepository,
            IPreOrderPolicyItemRepository preOrderPolicyItemRepository
        )
        {
            _productVariantRepository = productVariantRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _paymentRepository = paymentRepository;
            _walletRepository = walletRepository;
            _shopRepository = shopRepository;
            _preOrderDetailRepository = preOrderDetailRepository;
            _preOrderPolicyItemRepository = preOrderPolicyItemRepository;
            _transactionCode = new TransactionCode();
        }

        public async Task<PreOrderStatusDto> CreateAsync(Guid userId, CreatePreOrderRequest request)
        {
            if (request == null)
                throw new ArgumentException("Request is required.");
            if (request.VariantId == Guid.Empty && request.ProductId == Guid.Empty)
                throw new ArgumentException("ProductId or VariantId is required.");
            if (request.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than 0.");
            if (string.IsNullOrWhiteSpace(request.ShippingAddress))
                throw new ArgumentException("ShippingAddress is required.");

            var (variant, policy) = await ResolveVariantAndPolicyAsync(request);

            var product = await _productRepository.GetByIdAsync(variant.ProductId);
            if (product == null)
                throw new InvalidOperationException("Product not found.");

            var shop = await _shopRepository.GetByIdAsync(product.ShopId);
            if (shop == null)
                throw new InvalidOperationException("Shop not found.");

            var reservedQty = await _preOrderDetailRepository.GetReservedQuantityByVariantAsync(
                variant.Id
            );
            if (
                policy.MaxPreOrderQty.HasValue
                && reservedQty + request.Quantity > policy.MaxPreOrderQty.Value
            )
                throw new InvalidOperationException("Pre-order quantity exceeds configured limit.");

            var depositPercent = request.DepositPercent ?? policy.DepositPercent ?? 30m;
            if (depositPercent <= 0 || depositPercent > 100)
                throw new InvalidOperationException("DepositPercent must be in range 1..100.");

            var totalAmount = variant.Price * request.Quantity;
            var depositAmount = Math.Round(
                totalAmount * (depositPercent / 100m),
                2,
                MidpointRounding.AwayFromZero
            );
            var remainingAmount = totalAmount - depositAmount;

            var expectedAvailableDate =
                request.ExpectedAvailableDate ?? DateTime.UtcNow.AddDays(policy.LeadTimeDays ?? 7);

            var order = new Order
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TotalAmount = totalAmount,
                ShippingAddress = request.ShippingAddress.Trim(),
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                OrderType = OrderTypePreOrder,
                PreOrderStatus = PreOrderStatusDepositPending,
            };
            await _orderRepository.AddAsync(order);

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ProductVariantId = variant.Id,
                ProductName = string.IsNullOrWhiteSpace(variant.VariantName)
                    ? product.Name
                    : variant.VariantName,
                Price = variant.Price,
                Quantity = request.Quantity,
            };
            await _orderItemRepository.AddRangeAsync(new List<OrderItem> { orderItem });

            var preOrderDetail = new PreOrderDetail
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                ShopId = shop.Id,
                ExpectedAvailableDate = expectedAvailableDate,
                DepositPercent = depositPercent,
                DepositAmount = depositAmount,
                RemainingAmount = remainingAmount,
                FinalPaymentDeadlineHours = 24,
                CreatedAt = DateTime.UtcNow,
            };
            await _preOrderDetailRepository.AddAsync(preOrderDetail);

            return new PreOrderStatusDto
            {
                PreOrderId = preOrderDetail.Id,
                OrderId = order.Id,
                UserId = order.UserId,
                ShopId = preOrderDetail.ShopId,
                Status = order.PreOrderStatus ?? PreOrderStatusDepositPending,
                DepositAmount = preOrderDetail.DepositAmount,
                RemainingAmount = preOrderDetail.RemainingAmount,
                TotalAmount = order.TotalAmount,
                Message = "Pre-order created. Please pay deposit to continue.",
            };
        }

        private async Task<(
            ProductVariant Variant,
            PreOrderPolicyItem Policy
        )> ResolveVariantAndPolicyAsync(CreatePreOrderRequest request)
        {
            if (request.VariantId != Guid.Empty)
            {
                var variant = await _productVariantRepository.GetByIdAsync(request.VariantId);
                if (variant == null)
                    throw new InvalidOperationException("Product variant not found.");

                if (request.ProductId != Guid.Empty && request.ProductId != variant.ProductId)
                    throw new InvalidOperationException("ProductId does not match VariantId.");

                var directPolicy = await _preOrderPolicyItemRepository.GetByVariantIdAsync(
                    variant.Id
                );
                if (directPolicy != null && directPolicy.AllowPreOrder)
                    return (variant, directPolicy);

                if (!IsVariantEligibleForDefaultPreOrder(variant))
                    throw new InvalidOperationException("This variant does not support pre-order.");

                // Fallback mặc định khi chưa cấu hình policy riêng cho variant.
                return (variant, BuildDefaultPolicy(variant.Id, request.DepositPercent));
            }

            return await ResolveByProductIdAsync(request.ProductId);
        }

        private async Task<(
            ProductVariant Variant,
            PreOrderPolicyItem Policy
        )> ResolveByProductIdAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
                throw new InvalidOperationException("Product not found.");

            var variants = (
                await _productVariantRepository.GetByProductIdAsync(productId)
            ).ToList();
            if (!variants.Any())
                throw new InvalidOperationException("This product has no variant for pre-order.");

            var policies = await _preOrderPolicyItemRepository.GetActiveByProductIdAsync(productId);
            if (policies.Count == 0)
            {
                var defaultVariant = variants.FirstOrDefault(IsVariantEligibleForDefaultPreOrder);
                if (defaultVariant == null)
                    throw new InvalidOperationException("This product does not support pre-order.");

                // Product-level fallback: không có policy thì dùng rule mặc định.
                return (defaultVariant, BuildDefaultPolicy(defaultVariant.Id, null));
            }

            var variantMap = variants.ToDictionary(v => v.Id, v => v);

            var selectedPolicy = policies.FirstOrDefault(p =>
                variantMap.TryGetValue(p.ProductVariantId, out var v)
                && (
                    string.IsNullOrWhiteSpace(v.Status)
                    || v.Status.Equals("Active", StringComparison.OrdinalIgnoreCase)
                )
            );

            selectedPolicy ??= policies.First();

            if (!variantMap.TryGetValue(selectedPolicy.ProductVariantId, out var selectedVariant))
                throw new InvalidOperationException(
                    "No eligible variant found for this product pre-order."
                );

            return (selectedVariant, selectedPolicy);
        }

        private static bool IsVariantEligibleForDefaultPreOrder(ProductVariant variant)
        {
            if (variant == null)
                return false;

            return string.IsNullOrWhiteSpace(variant.Status)
                || variant.Status.Equals("Active", StringComparison.OrdinalIgnoreCase)
                || variant.Status.Equals("InStock", StringComparison.OrdinalIgnoreCase)
                || variant.Status.Equals("Available", StringComparison.OrdinalIgnoreCase);
        }

        private static PreOrderPolicyItem BuildDefaultPolicy(
            Guid variantId,
            decimal? requestDepositPercent
        )
        {
            return new PreOrderPolicyItem
            {
                Id = Guid.Empty,
                ProductVariantId = variantId,
                AllowPreOrder = true,
                DepositPercent = requestDepositPercent,
                MaxPreOrderQty = null,
                LeadTimeDays = 7,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
            };
        }

        public async Task<PreOrderStatusDto> PayDepositAsync(
            Guid userId,
            PayPreOrderDepositRequest request
        )
        {
            var detail =
                await _preOrderDetailRepository.GetByIdWithOrderAsync(request.PreOrderId)
                ?? throw new InvalidOperationException("Pre-order not found.");

            if (detail.Order.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this pre-order.");

            if (
                !string.Equals(
                    detail.Order.PreOrderStatus,
                    PreOrderStatusDepositPending,
                    StringComparison.OrdinalIgnoreCase
                )
            )
                throw new InvalidOperationException(
                    "Pre-order is not waiting for deposit payment."
                );

            await ProcessPaymentAsync(
                userId,
                detail.Order,
                request.PaymentMethod,
                detail.DepositAmount,
                "DEPOSIT"
            );

            detail.Order.PreOrderStatus = PreOrderStatusDepositPaid;
            detail.Order.Status = "Pending";
            await _orderRepository.UpdateAsync(detail.Order);

            return new PreOrderStatusDto
            {
                PreOrderId = detail.Id,
                OrderId = detail.OrderId,
                UserId = detail.Order.UserId,
                ShopId = detail.ShopId,
                Status = detail.Order.PreOrderStatus,
                DepositAmount = detail.DepositAmount,
                RemainingAmount = detail.RemainingAmount,
                TotalAmount = detail.Order.TotalAmount,
                Message = "Deposit paid successfully.",
            };
        }

        public async Task<PreOrderStatusDto> MarkReadyForFinalPaymentAsync(
            Guid shopUserId,
            MarkPreOrderReadyRequest request
        )
        {
            var shop =
                await _shopRepository.GetByUserIdAsync(shopUserId)
                ?? throw new UnauthorizedAccessException("Shop account is required.");

            var detail =
                await _preOrderDetailRepository.GetByIdWithOrderAsync(request.PreOrderId)
                ?? throw new InvalidOperationException("Pre-order not found.");

            if (detail.ShopId != shop.Id)
                throw new UnauthorizedAccessException("You cannot update this pre-order.");

            if (
                !string.Equals(
                    detail.Order.PreOrderStatus,
                    PreOrderStatusDepositPaid,
                    StringComparison.OrdinalIgnoreCase
                )
            )
                throw new InvalidOperationException("Pre-order has not completed deposit payment.");

            var deadlineHours = 24;
            detail.FinalPaymentDeadlineHours = deadlineHours;
            detail.ActivatedFinalPaymentAt = DateTime.UtcNow;
            detail.UpdatedAt = DateTime.UtcNow;
            await _preOrderDetailRepository.UpdateAsync(detail);

            detail.Order.PreOrderStatus = PreOrderStatusReadyForFinalPayment;
            detail.Order.FinalPaymentDueAt = DateTime.UtcNow.AddHours(deadlineHours);
            await _orderRepository.UpdateAsync(detail.Order);

            return new PreOrderStatusDto
            {
                PreOrderId = detail.Id,
                OrderId = detail.OrderId,
                UserId = detail.Order.UserId,
                ShopId = detail.ShopId,
                Status = detail.Order.PreOrderStatus,
                DepositAmount = detail.DepositAmount,
                RemainingAmount = detail.RemainingAmount,
                TotalAmount = detail.Order.TotalAmount,
                FinalPaymentDeadline = detail.Order.FinalPaymentDueAt,
                Message = "Pre-order is now ready for final payment.",
            };
        }

        public async Task<PreOrderStatusDto> PayRemainingAsync(
            Guid userId,
            FinalizePreOrderPaymentRequest request
        )
        {
            var detail =
                await _preOrderDetailRepository.GetByIdWithOrderAsync(request.PreOrderId)
                ?? throw new InvalidOperationException("Pre-order not found.");

            if (detail.Order.UserId != userId)
                throw new UnauthorizedAccessException("You do not own this pre-order.");

            if (
                !string.Equals(
                    detail.Order.PreOrderStatus,
                    PreOrderStatusReadyForFinalPayment,
                    StringComparison.OrdinalIgnoreCase
                )
            )
                throw new InvalidOperationException("Pre-order is not ready for final payment.");

            if (
                detail.Order.FinalPaymentDueAt.HasValue
                && DateTime.UtcNow > detail.Order.FinalPaymentDueAt.Value
            )
            {
                detail.Order.PreOrderStatus = PreOrderStatusExpired;
                detail.ExpiredAt = DateTime.UtcNow;
                detail.UpdatedAt = DateTime.UtcNow;
                await _preOrderDetailRepository.UpdateAsync(detail);
                await _orderRepository.UpdateAsync(detail.Order);
                throw new InvalidOperationException("Final payment deadline has passed.");
            }

            await ProcessPaymentAsync(
                userId,
                detail.Order,
                request.PaymentMethod,
                detail.RemainingAmount,
                "FINAL"
            );

            detail.Order.PreOrderStatus = PreOrderStatusCompleted;
            detail.Order.Status = "Processing";
            detail.Order.FinalPaymentDueAt = null;
            await _orderRepository.UpdateAsync(detail.Order);

            return new PreOrderStatusDto
            {
                PreOrderId = detail.Id,
                OrderId = detail.OrderId,
                UserId = detail.Order.UserId,
                ShopId = detail.ShopId,
                Status = detail.Order.PreOrderStatus,
                DepositAmount = detail.DepositAmount,
                RemainingAmount = 0,
                TotalAmount = detail.Order.TotalAmount,
                Message = "Final payment completed. Order moved to processing.",
            };
        }

        public async Task<List<PreOrderSummaryDto>> GetMyPreOrdersAsync(Guid userId)
        {
            var details = await _preOrderDetailRepository.GetByUserIdAsync(userId);
            return details.Select(MapToSummary).ToList();
        }

        public async Task<List<PreOrderSummaryDto>> GetShopPreOrdersAsync(Guid shopUserId)
        {
            var shop =
                await _shopRepository.GetByUserIdAsync(shopUserId)
                ?? throw new UnauthorizedAccessException("Shop account is required.");

            var details = await _preOrderDetailRepository.GetByShopIdAsync(shop.Id);
            return details.Select(MapToSummary).ToList();
        }

        public async Task<int> ExpireOverduePreOrdersAsync()
        {
            var expiredCount = 0;
            var now = DateTime.UtcNow;
            // Repository currently provides filtered lists by user/shop only, so query by shop pages is not suitable.
            // This batch expiration intentionally uses shop list aggregation by all shops.
            var allShops = await _shopRepository.GetAllAsync();
            foreach (var shop in allShops)
            {
                var shopPreOrders = await _preOrderDetailRepository.GetByShopIdAsync(shop.Id);
                foreach (var detail in shopPreOrders)
                {
                    if (
                        !string.Equals(
                            detail.Order.PreOrderStatus,
                            PreOrderStatusReadyForFinalPayment,
                            StringComparison.OrdinalIgnoreCase
                        )
                    )
                        continue;
                    if (
                        !detail.Order.FinalPaymentDueAt.HasValue
                        || detail.Order.FinalPaymentDueAt.Value >= now
                    )
                        continue;

                    detail.Order.PreOrderStatus = PreOrderStatusExpired;
                    detail.ExpiredAt = now;
                    detail.UpdatedAt = now;
                    await _preOrderDetailRepository.UpdateAsync(detail);
                    await _orderRepository.UpdateAsync(detail.Order);
                    expiredCount++;
                }
            }

            return expiredCount;
        }

        private async Task ProcessPaymentAsync(
            Guid userId,
            Order order,
            string paymentMethod,
            decimal amount,
            string stage
        )
        {
            if (amount <= 0)
                throw new InvalidOperationException("Invalid payment amount.");

            var normalizedMethod = string.IsNullOrWhiteSpace(paymentMethod)
                ? "WALLET"
                : paymentMethod.Trim().ToUpperInvariant();

            if (normalizedMethod == "WALLET")
            {
                var wallet = await GetOrCreateWalletAsync(userId);
                if (wallet.Balance < amount)
                    throw new InvalidOperationException(
                        "Insufficient wallet balance. Please top up your wallet and try again."
                    );

                wallet.Balance -= amount;
                wallet.LastChangeAmount = -amount;
                wallet.LastChangeType = $"PreOrder {stage}";
                wallet.UpdatedAt = DateTime.UtcNow;
                await _walletRepository.UpdateAsync(wallet);
            }

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                OrderId = order.Id,
                Method = normalizedMethod,
                PaymentStage = stage,
                Amount = amount,
                Status = "Paid",
                TransactionCode = _transactionCode.GenerateTransactionCode(order.Id),
                PaidAt = DateTime.UtcNow,
            };
            await _paymentRepository.AddAsync(payment);
        }

        private async Task<Wallet> GetOrCreateWalletAsync(Guid userId)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            if (wallet != null)
                return wallet;

            wallet = new Wallet
            {
                WalletId = Guid.NewGuid(),
                UserId = userId,
                Balance = 0m,
                LastChangeAmount = 0m,
                LastChangeType = "Wallet initialized",
                UpdatedAt = DateTime.UtcNow,
            };

            await _walletRepository.AddAsync(wallet);
            return wallet;
        }

        private static PreOrderSummaryDto MapToSummary(PreOrderDetail detail)
        {
            var firstItem = detail.Order.OrderItems.FirstOrDefault();
            return new PreOrderSummaryDto
            {
                PreOrderId = detail.Id,
                OrderId = detail.OrderId,
                UserId = detail.Order.UserId,
                ShopId = detail.ShopId,
                PreOrderStatus = detail.Order.PreOrderStatus ?? string.Empty,
                OrderStatus = detail.Order.Status,
                DepositAmount = detail.DepositAmount,
                RemainingAmount = detail.RemainingAmount,
                TotalAmount = detail.Order.TotalAmount,
                ProductName = firstItem?.ProductName ?? string.Empty,
                Quantity = firstItem?.Quantity ?? 0,
                ExpectedAvailableDate = detail.ExpectedAvailableDate,
                FinalPaymentDueAt = detail.Order.FinalPaymentDueAt,
                CreatedAt = detail.CreatedAt,
            };
        }
    }
}
