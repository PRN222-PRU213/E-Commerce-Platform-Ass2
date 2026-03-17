using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Models;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    /// <summary>
    /// Service xử lý đơn hàng cho Shop Owner
    /// </summary>
    public class ShopOrderService : IShopOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IUserRepository _userRepository;

        public ShopOrderService(
            IOrderRepository orderRepository,
            IShipmentRepository shipmentRepository,
            IUserRepository userRepository
        )
        {
            _orderRepository = orderRepository;
            _shipmentRepository = shipmentRepository;
            _userRepository = userRepository;
        }

        public async Task<ServiceResult<List<OrderDto>>> GetOrdersByShopIdAsync(Guid shopId)
        {
            var orders = await _orderRepository.GetByShopIdAsync(shopId);

            var orderDtos = orders
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    UserId = o.UserId,
                    CustomerName = o.User?.Name,
                    CustomerEmail = o.User?.Email,
                    TotalAmount = o.TotalAmount,
                    ShippingAddress = o.ShippingAddress,
                    Status = o.Status,
                    OrderType = o.OrderType,
                    PreOrderStatus = o.PreOrderStatus,
                    CreatedAt = o.CreatedAt,
                    ItemCount = o.OrderItems.Count,
                    Carrier = o.Shipments?.FirstOrDefault()?.Carrier,
                    TrackingCode = o.Shipments?.FirstOrDefault()?.TrackingCode,
                    ShipmentStatus = o.Shipments?.FirstOrDefault()?.Status,
                })
                .ToList();

            return ServiceResult<List<OrderDto>>.Success(orderDtos);
        }

        public async Task<ServiceResult<OrderDetailDto>> GetOrderDetailAsync(
            Guid orderId,
            Guid shopId
        )
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
            if (order == null)
            {
                return ServiceResult<OrderDetailDto>.Failure("Đơn hàng không tồn tại.");
            }

            // Kiểm tra đơn hàng có thuộc shop này không
            var hasShopItem = order.OrderItems.Any(oi =>
                oi.ProductVariant?.Product?.ShopId == shopId
            );
            if (!hasShopItem)
            {
                return ServiceResult<OrderDetailDto>.Failure("Đơn hàng không thuộc shop của bạn.");
            }

            var shipment = order.Shipments?.FirstOrDefault();
            var payment = order.Payments?.FirstOrDefault();
            var payments = order.Payments ?? new List<Payment>();
            var preOrderDepositedAmount = payments
                .Where(p =>
                    string.Equals(p.Status, "Paid", StringComparison.OrdinalIgnoreCase)
                    && string.Equals(p.PaymentStage, "DEPOSIT", StringComparison.OrdinalIgnoreCase)
                )
                .Sum(p => p.Amount);

            var preOrderRemainingAmount = string.Equals(
                order.PreOrderStatus,
                "COMPLETED",
                StringComparison.OrdinalIgnoreCase
            )
                ? 0m
                : Math.Max(0m, order.TotalAmount - preOrderDepositedAmount);

            var dto = new OrderDetailDto
            {
                Id = order.Id,
                UserId = order.UserId,
                CustomerName = order.User?.Name,
                CustomerEmail = order.User?.Email,
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                Status = order.Status,
                OrderType = order.OrderType,
                PreOrderStatus = order.PreOrderStatus,
                PreOrderDepositedAmount = preOrderDepositedAmount,
                PreOrderRemainingAmount = preOrderRemainingAmount,
                CreatedAt = order.CreatedAt,
                ItemCount = order.OrderItems.Count,
                Carrier = shipment?.Carrier,
                TrackingCode = shipment?.TrackingCode,
                ShipmentStatus = shipment?.Status,
                Items = order
                    .OrderItems.Select(oi => new OrderItemDto
                    {
                        Id = oi.Id,
                        ProductVariantId = oi.ProductVariantId,
                        ProductName = oi.ProductName,
                        VariantName = oi.ProductVariant?.VariantName,
                        Size = oi.ProductVariant?.Size,
                        Color = oi.ProductVariant?.Color,
                        ImageUrl = oi.ProductVariant?.Product?.ImageUrl,
                        Price = oi.Price,
                        Quantity = oi.Quantity,
                    })
                    .ToList(),
                Payment =
                    payment != null
                        ? new PaymentDto
                        {
                            Id = payment.Id,
                            Method = payment.Method,
                            Amount = payment.Amount,
                            Status = payment.Status,
                            TransactionCode = payment.TransactionCode,
                            PaidAt = payment.PaidAt,
                        }
                        : null,
                Shipment =
                    shipment != null
                        ? new ShipmentDto
                        {
                            Id = shipment.Id,
                            Carrier = shipment.Carrier,
                            TrackingCode = shipment.TrackingCode,
                            Status = shipment.Status,
                            DeliveryAttemptCount = shipment.DeliveryAttemptCount,
                            UpdatedAt = shipment.UpdatedAt,
                        }
                        : null,
            };

            return ServiceResult<OrderDetailDto>.Success(dto);
        }

        /// <summary>
        /// Bắt đầu xử lý đơn hàng: Pending → Processing
        /// </summary>
        public async Task<ServiceResult> StartProcessingAsync(Guid orderId, Guid shopId)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(orderId);
            if (order == null)
            {
                return ServiceResult.Failure("Đơn hàng không tồn tại.");
            }

            // Kiểm tra quyền
            var hasShopItem = order.OrderItems.Any(oi =>
                oi.ProductVariant?.Product?.ShopId == shopId
            );
            if (!hasShopItem)
            {
                return ServiceResult.Failure("Đơn hàng không thuộc shop của bạn.");
            }

            var preorderGuard = EnsurePreOrderCanBeProcessed(order);
            if (!preorderGuard.IsSuccess)
            {
                return preorderGuard;
            }

            // Chấp nhận Pending hoặc PAID (từ checkout cũ)
            var status = order.Status?.Trim().ToLower();
            if (status != "pending" && status != "paid")
            {
                return ServiceResult.Failure(
                    $"Chỉ có thể xử lý đơn hàng đang chờ. Status hiện tại: {order.Status}"
                );
            }

            order.Status = "Processing";
            await _orderRepository.UpdateAsync(order);

            return ServiceResult.Success();
        }

        /// <summary>
        /// Chuyển sang chuẩn bị hàng: Processing → Preparing
        /// </summary>
        public async Task<ServiceResult> StartPreparingAsync(Guid orderId, Guid shopId)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(orderId);
            if (order == null)
            {
                return ServiceResult.Failure("Đơn hàng không tồn tại.");
            }

            // Kiểm tra quyền
            var hasShopItem = order.OrderItems.Any(oi =>
                oi.ProductVariant?.Product?.ShopId == shopId
            );
            if (!hasShopItem)
            {
                return ServiceResult.Failure("Đơn hàng không thuộc shop của bạn.");
            }

            var preorderGuard = EnsurePreOrderCanBeProcessed(order);
            if (!preorderGuard.IsSuccess)
            {
                return preorderGuard;
            }

            var status = order.Status?.Trim().ToLower();
            if (status != "processing")
            {
                return ServiceResult.Failure(
                    $"Chỉ có thể chuẩn bị hàng khi đơn đang xử lý. Status hiện tại: {order.Status}"
                );
            }

            order.Status = "Preparing";
            await _orderRepository.UpdateAsync(order);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> ConfirmOrderAsync(Guid orderId, Guid shopId)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(orderId);
            if (order == null)
            {
                return ServiceResult.Failure("Đơn hàng không tồn tại.");
            }

            // Kiểm tra quyền
            var hasShopItem = order.OrderItems.Any(oi =>
                oi.ProductVariant?.Product?.ShopId == shopId
            );
            if (!hasShopItem)
            {
                return ServiceResult.Failure("Đơn hàng không thuộc shop của bạn.");
            }

            var preorderGuard = EnsurePreOrderCanBeProcessed(order);
            if (!preorderGuard.IsSuccess)
            {
                return preorderGuard;
            }

            var statusConfirm = order.Status?.Trim().ToLower();
            if (statusConfirm != "pending" && statusConfirm != "paid")
            {
                return ServiceResult.Failure(
                    $"Chỉ có thể xác nhận đơn hàng đang chờ xử lý. Status hiện tại: {order.Status}"
                );
            }

            order.Status = "Confirmed";
            await _orderRepository.UpdateAsync(order);

            return ServiceResult.Success();
        }

        /// <summary>
        /// Gửi hàng - Tạo shipment và chuyển trạng thái đơn hàng sang Shipped
        /// </summary>
        public async Task<ServiceResult> ShipOrderAsync(
            Guid orderId,
            Guid shopId,
            CreateShipmentDto dto
        )
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
            if (order == null)
            {
                return ServiceResult.Failure("Đơn hàng không tồn tại.");
            }

            // Kiểm tra quyền
            var hasShopItem = order.OrderItems.Any(oi =>
                oi.ProductVariant?.Product?.ShopId == shopId
            );
            if (!hasShopItem)
            {
                return ServiceResult.Failure("Đơn hàng không thuộc shop của bạn.");
            }

            var preorderGuard = EnsurePreOrderCanBeProcessed(order);
            if (!preorderGuard.IsSuccess)
            {
                return preorderGuard;
            }

            var statusShip = order.Status?.Trim().ToLower();
            if (
                statusShip != "preparing"
                && statusShip != "confirmed"
                && statusShip != "deliveryfailed"
            )
            {
                return ServiceResult.Failure(
                    $"Chỉ có thể gửi hàng khi đơn đang chuẩn bị hoặc sau khi giao thất bại. Status hiện tại: {order.Status}"
                );
            }

            // Validate input
            if (string.IsNullOrWhiteSpace(dto.Carrier))
            {
                return ServiceResult.Failure("Vui lòng chọn đơn vị vận chuyển.");
            }
            if (string.IsNullOrWhiteSpace(dto.TrackingCode))
            {
                return ServiceResult.Failure("Vui lòng nhập mã vận chuyển đơn.");
            }

            var carrier = dto.Carrier.Trim();
            var trackingCode = dto.TrackingCode.Trim();
            var existingShipment = order.Shipments?.FirstOrDefault();

            if (existingShipment == null)
            {
                var existsCode = await _shipmentRepository.ExistsTrackingCodeAsync(trackingCode);
                if (existsCode)
                {
                    return ServiceResult.Failure(
                        "Vui lòng nhập mã vận chuyển khác vì đã bị trùng."
                    );
                }

                var shipment = new Shipment
                {
                    Id = Guid.NewGuid(),
                    OrderId = orderId,
                    Carrier = carrier,
                    TrackingCode = trackingCode,
                    Status = "Shipping",
                    DeliveryAttemptCount = 1,
                    UpdatedAt = DateTime.Now,
                };
                await _shipmentRepository.AddAsync(shipment);
            }
            else
            {
                var sameTrackingCode = string.Equals(
                    existingShipment.TrackingCode,
                    trackingCode,
                    StringComparison.OrdinalIgnoreCase
                );

                if (!sameTrackingCode)
                {
                    var existsCode = await _shipmentRepository.ExistsTrackingCodeAsync(
                        trackingCode
                    );
                    if (existsCode)
                    {
                        return ServiceResult.Failure(
                            "Vui lòng nhập mã vận chuyển khác vì đã bị trùng."
                        );
                    }
                }

                if (statusShip == "deliveryfailed")
                {
                    if (existingShipment.DeliveryAttemptCount >= 3)
                    {
                        return ServiceResult.Failure(
                            "Đơn hàng đã vượt quá 3 lần giao. Không thể giao lại."
                        );
                    }

                    existingShipment.DeliveryAttemptCount += 1;
                }

                if (existingShipment.DeliveryAttemptCount <= 0)
                {
                    existingShipment.DeliveryAttemptCount = 1;
                }

                existingShipment.Carrier = carrier;
                existingShipment.TrackingCode = trackingCode;
                existingShipment.Status = "Shipping";
                existingShipment.UpdatedAt = DateTime.Now;
                await _shipmentRepository.UpdateAsync(existingShipment);
            }

            // Cập nhật order status
            order.Status = "Shipped";
            await _orderRepository.UpdateAsync(order);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> UpdateShipmentAsync(
            Guid orderId,
            Guid shopId,
            UpdateShipmentDto dto
        )
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
            if (order == null)
            {
                return ServiceResult.Failure("Đơn hàng không tồn tại.");
            }

            // Kiểm tra quyền
            var hasShopItem = order.OrderItems.Any(oi =>
                oi.ProductVariant?.Product?.ShopId == shopId
            );
            if (!hasShopItem)
            {
                return ServiceResult.Failure("Đơn hàng không thuộc shop của bạn.");
            }

            var preorderGuard = EnsurePreOrderCanBeProcessed(order);
            if (!preorderGuard.IsSuccess)
            {
                return preorderGuard;
            }

            var statusUpdate = order.Status?.Trim().ToLower();
            if (statusUpdate != "shipped" && statusUpdate != "shipping")
            {
                return ServiceResult.Failure(
                    $"Chỉ có thể cập nhật vận chuyển khi đơn đang giao. Status hiện tại: {order.Status}"
                );
            }

            // Cập nhật shipment
            var shipment = order.Shipments?.FirstOrDefault();
            if (shipment == null)
            {
                return ServiceResult.Failure("Không tìm thấy thông tin vận chuyển.");
            }

            shipment.Carrier = dto.Carrier;
            shipment.TrackingCode = dto.TrackingCode;
            shipment.Status = dto.Status;
            shipment.UpdatedAt = DateTime.Now;
            await _shipmentRepository.UpdateAsync(shipment);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> MarkAsDeliveredAsync(Guid orderId, Guid shopId)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
            if (order == null)
            {
                return ServiceResult.Failure("Đơn hàng không tồn tại.");
            }

            // Kiểm tra quyền
            var hasShopItem = order.OrderItems.Any(oi =>
                oi.ProductVariant?.Product?.ShopId == shopId
            );
            if (!hasShopItem)
            {
                return ServiceResult.Failure("Đơn hàng không thuộc shop của bạn.");
            }

            var preorderGuard = EnsurePreOrderCanBeProcessed(order);
            if (!preorderGuard.IsSuccess)
            {
                return preorderGuard;
            }

            // Kiểm tra trạng thái - chỉ cho phép khi đang shipped
            var statusDelivered = order.Status?.Trim().ToLower();
            if (statusDelivered != "shipped" && statusDelivered != "shipping")
            {
                return ServiceResult.Failure(
                    $"Chỉ có thể đánh dấu giao hàng cho đơn hàng đang vận chuyển. Status hiện tại: {order.Status}"
                );
            }

            // Cập nhật order
            order.Status = "Completed";
            await _orderRepository.UpdateAsync(order);

            // Cập nhật shipment
            var shipment = order.Shipments?.FirstOrDefault();
            if (shipment != null)
            {
                shipment.Status = "Delivered";
                shipment.UpdatedAt = DateTime.Now;
                await _shipmentRepository.UpdateAsync(shipment);
            }

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> MarkAsDeliveryFailedAsync(Guid orderId, Guid shopId)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
            if (order == null)
            {
                return ServiceResult.Failure("Đơn hàng không tồn tại.");
            }

            var hasShopItem = order.OrderItems.Any(oi =>
                oi.ProductVariant?.Product?.ShopId == shopId
            );
            if (!hasShopItem)
            {
                return ServiceResult.Failure("Đơn hàng không thuộc shop của bạn.");
            }

            var preorderGuard = EnsurePreOrderCanBeProcessed(order);
            if (!preorderGuard.IsSuccess)
            {
                return preorderGuard;
            }

            var status = order.Status?.Trim().ToLower();
            if (status != "shipped" && status != "shipping")
            {
                return ServiceResult.Failure(
                    $"Chỉ có thể đánh dấu giao hàng thất bại khi đơn đang vận chuyển. Status hiện tại: {order.Status}"
                );
            }

            var shipment = order.Shipments?.FirstOrDefault();
            if (shipment != null)
            {
                if (shipment.DeliveryAttemptCount <= 0)
                {
                    shipment.DeliveryAttemptCount = 1;
                }

                shipment.Status = "DeliveryFailed";
                shipment.UpdatedAt = DateTime.Now;
                await _shipmentRepository.UpdateAsync(shipment);

                if (shipment.DeliveryAttemptCount >= 3)
                {
                    order.Status = "Cancelled";
                }
                else
                {
                    order.Status = "DeliveryFailed";
                }
            }
            else
            {
                order.Status = "DeliveryFailed";
            }

            await _orderRepository.UpdateAsync(order);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> RejectOrderAsync(Guid orderId, Guid shopId, string? reason)
        {
            var order = await _orderRepository.GetByIdWithItemsAsync(orderId);
            if (order == null)
            {
                return ServiceResult.Failure("Đơn hàng không tồn tại.");
            }

            // Kiểm tra quyền
            var hasShopItem = order.OrderItems.Any(oi =>
                oi.ProductVariant?.Product?.ShopId == shopId
            );
            if (!hasShopItem)
            {
                return ServiceResult.Failure("Đơn hàng không thuộc shop của bạn.");
            }

            var statusReject = order.Status?.Trim().ToLower();
            if (statusReject != "pending" && statusReject != "paid")
            {
                return ServiceResult.Failure(
                    $"Chỉ có thể từ chối đơn hàng đang chờ xử lý. Status hiện tại: {order.Status}"
                );
            }

            order.Status = "Cancelled";
            await _orderRepository.UpdateAsync(order);

            return ServiceResult.Success();
        }

        private static ServiceResult EnsurePreOrderCanBeProcessed(Order order)
        {
            if (!string.Equals(order.OrderType, "PreOrder", StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult.Success();
            }

            if (
                string.Equals(order.PreOrderStatus, "COMPLETED", StringComparison.OrdinalIgnoreCase)
            )
            {
                return ServiceResult.Success();
            }

            return ServiceResult.Failure(
                "Đơn pre-order chưa thanh toán đầy đủ. Shop chỉ có thể lên đơn khi khách đã thanh toán đủ (COMPLETED)."
            );
        }

        public async Task<ShopOrderStatistics> GetOrderStatisticsAsync(Guid shopId)
        {
            var orders = await _orderRepository.GetByShopIdAsync(shopId);
            var orderList = orders.ToList();

            return new ShopOrderStatistics
            {
                TotalOrders = orderList.Count,
                PendingOrders = orderList.Count(o =>
                    o.Status?.ToLower() == "pending" || o.Status?.ToLower() == "paid"
                ),
                ConfirmedOrders = orderList.Count(o =>
                    o.Status?.ToLower() == "processing"
                    || o.Status?.ToLower() == "preparing"
                    || o.Status?.ToLower() == "confirmed"
                ),
                ShippingOrders = orderList.Count(o =>
                    o.Status?.ToLower() == "shipped" || o.Status?.ToLower() == "shipping"
                ),
                DeliveredOrders = orderList.Count(o =>
                    o.Status?.ToLower() == "completed" || o.Status?.ToLower() == "delivered"
                ),
                CancelledOrders = orderList.Count(o =>
                    o.Status?.ToLower() == "cancelled"
                    || o.Status?.ToLower() == "rejected"
                    || o.Status?.ToLower() == "deliveryfailed"
                ),
                TotalRevenue = orderList
                    .Where(o =>
                        o.Status?.ToLower() == "completed" || o.Status?.ToLower() == "delivered"
                    )
                    .Sum(o => o.TotalAmount),
            };
        }
    }
}
