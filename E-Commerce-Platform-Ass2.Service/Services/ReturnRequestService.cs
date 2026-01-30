using System.Text.Json;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Models;
using E_Commerce_Platform_Ass2.Service.Options;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.Extensions.Options;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class ReturnRequestService : IReturnRequestService
    {
        private readonly IReturnRequestRepository _returnRequestRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IRefundService _refundService;
        private readonly IShopWalletService _shopWalletService;
        private readonly RefundBusinessRules _rules;

        public ReturnRequestService(
            IReturnRequestRepository returnRequestRepository,
            IOrderRepository orderRepository,
            IRefundService refundService,
            IShopWalletService shopWalletService,
            IOptions<RefundBusinessRules> rulesOptions)
        {
            _returnRequestRepository = returnRequestRepository;
            _orderRepository = orderRepository;
            _refundService = refundService;
            _shopWalletService = shopWalletService;
            _rules = rulesOptions.Value;
        }

        #region Customer Methods

        public async Task<ServiceResult<ReturnRequestDto>> CreateRequestAsync(CreateReturnRequestDto dto)
        {
            // Validate order exists and belongs to user
            var order = await _orderRepository.GetByIdAsync(dto.OrderId);
            if (order == null)
            {
                return ServiceResult<ReturnRequestDto>.Failure("Không tìm thấy đơn hàng.");
            }

            if (order.UserId != dto.UserId)
            {
                return ServiceResult<ReturnRequestDto>.Failure("Bạn không có quyền yêu cầu hoàn trả đơn hàng này.");
            }

            // Check order status - only allow for Completed or Shipped
            if (order.Status != "Completed" && order.Status != "Shipped")
            {
                return ServiceResult<ReturnRequestDto>.Failure($"Không thể yêu cầu hoàn trả cho đơn hàng ở trạng thái '{order.Status}'.");
            }

            // ⭐ BUSINESS RULE: Kiểm tra thời hạn hoàn tiền
            var completedDate = order.CompletedAt ?? order.CreatedAt;
            var daysSinceCompleted = (DateTime.UtcNow - completedDate).Days;
            var deadlineDays = dto.RequestType == "Return" ? _rules.ReturnDeadlineDays : _rules.RefundDeadlineDays;
            if (daysSinceCompleted > deadlineDays)
            {
                return ServiceResult<ReturnRequestDto>.Failure(
                    $"Đã quá thời hạn {deadlineDays} ngày để yêu cầu {(dto.RequestType == "Return" ? "đổi trả" : "hoàn tiền")}. Bạn không thể thực hiện yêu cầu này nữa.");
            }

            // ⭐ BUSINESS RULE: Kiểm tra số lần yêu cầu trên đơn hàng
            var orderRequestCount = await _returnRequestRepository.CountByOrderIdAsync(dto.OrderId);
            if (orderRequestCount >= _rules.MaxRefundPerOrder)
            {
                return ServiceResult<ReturnRequestDto>.Failure(
                    $"Đơn hàng này đã đạt giới hạn {_rules.MaxRefundPerOrder} lần yêu cầu hoàn tiền.");
            }

            // ⭐ BUSINESS RULE: Kiểm tra số lần yêu cầu trong tháng
            var monthlyRequestCount = await _returnRequestRepository.CountByUserIdThisMonthAsync(dto.UserId);
            if (monthlyRequestCount >= _rules.MaxRefundPerMonth)
            {
                return ServiceResult<ReturnRequestDto>.Failure(
                    $"Bạn đã đạt giới hạn {_rules.MaxRefundPerMonth} yêu cầu hoàn tiền trong tháng này.");
            }

            // Check if request already exists (not rejected)
            var existingRequest = await _returnRequestRepository.ExistsByOrderIdAsync(dto.OrderId);
            if (existingRequest)
            {
                return ServiceResult<ReturnRequestDto>.Failure("Đơn hàng này đã có yêu cầu hoàn trả đang được xử lý.");
            }

            // Validate amount
            if (dto.RequestedAmount <= 0 || dto.RequestedAmount > order.TotalAmount)
            {
                return ServiceResult<ReturnRequestDto>.Failure($"Số tiền yêu cầu phải từ 1 đến {order.TotalAmount:N0} VNĐ.");
            }

            var request = new ReturnRequest
            {
                Id = Guid.NewGuid(),
                OrderId = dto.OrderId,
                UserId = dto.UserId,
                RequestType = dto.RequestType,
                Reason = dto.Reason,
                ReasonDetail = dto.ReasonDetail,
                EvidenceImages = dto.EvidenceImageUrls != null && dto.EvidenceImageUrls.Any()
                    ? JsonSerializer.Serialize(dto.EvidenceImageUrls)
                    : null,
                RequestedAmount = dto.RequestedAmount,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            await _returnRequestRepository.AddAsync(request);

            return ServiceResult<ReturnRequestDto>.Success(MapToDto(request, order, null));
        }

        public async Task<IEnumerable<ReturnRequestDto>> GetMyRequestsAsync(Guid userId)
        {
            var requests = await _returnRequestRepository.GetByUserIdAsync(userId);
            return requests.Select(r => MapToDto(r, r.Order, null));
        }

        public async Task<ReturnRequestDto?> GetRequestDetailAsync(Guid requestId, Guid userId)
        {
            var request = await _returnRequestRepository.GetByIdWithDetailsAsync(requestId);
            if (request == null || request.UserId != userId)
            {
                return null;
            }

            return MapToDto(request, request.Order, request.ProcessedByShop);
        }

        public async Task<bool> CanCreateRequestAsync(Guid orderId, Guid userId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null || order.UserId != userId)
            {
                return false;
            }

            if (order.Status != "Completed" && order.Status != "Shipped")
            {
                return false;
            }

            var existingRequest = await _returnRequestRepository.ExistsByOrderIdAsync(orderId);
            return !existingRequest;
        }

        #endregion

        #region Shop Methods

        public async Task<IEnumerable<ReturnRequestDto>> GetShopRequestsAsync(Guid shopId, string? status = null)
        {
            var requests = await _returnRequestRepository.GetByShopIdAsync(shopId, status);
            return requests.Select(r => MapToDto(r, r.Order, r.ProcessedByShop));
        }

        public async Task<ReturnRequestDto?> GetShopRequestDetailAsync(Guid requestId, Guid shopId)
        {
            var request = await _returnRequestRepository.GetByIdWithDetailsAsync(requestId);
            if (request == null)
            {
                return null;
            }

            // Verify this request belongs to an order from this shop
            var belongsToShop = request.Order.OrderItems
                .Any(oi => oi.ProductVariant?.Product?.ShopId == shopId);

            if (!belongsToShop)
            {
                return null;
            }

            return MapToDto(request, request.Order, request.ProcessedByShop);
        }

        public async Task<ServiceResult> ApproveRequestAsync(Guid requestId, Guid shopId, decimal? approvedAmount, string? response)
        {
            var request = await _returnRequestRepository.GetByIdWithDetailsAsync(requestId);
            if (request == null)
            {
                return ServiceResult.Failure("Không tìm thấy yêu cầu hoàn trả.");
            }

            if (request.Status != "Pending")
            {
                return ServiceResult.Failure("Yêu cầu này đã được xử lý.");
            }

            // Verify shop ownership
            var belongsToShop = request.Order.OrderItems
                .Any(oi => oi.ProductVariant?.Product?.ShopId == shopId);

            if (!belongsToShop)
            {
                return ServiceResult.Failure("Bạn không có quyền xử lý yêu cầu này.");
            }

            var refundAmount = approvedAmount ?? request.RequestedAmount;

            // Validate amount
            if (refundAmount <= 0 || refundAmount > request.Order.TotalAmount)
            {
                return ServiceResult.Failure("Số tiền hoàn trả không hợp lệ.");
            }

            // ⭐ BUSINESS RULE: Tính phí hoàn tiền dựa trên lý do
            var isShopFault = _rules.IsShopFault(request.Reason);
            var refundFee = _rules.CalculateFee(refundAmount, request.Reason);
            var finalRefundAmount = refundAmount - refundFee;

            try
            {
                // Gọi RefundService để hoàn tiền về ví khách hàng (TÁI SỬ DỤNG)
                // Hoàn số tiền sau khi trừ phí
                await _refundService.RefundAsync(
                    request.OrderId,
                    finalRefundAmount,
                    $"Hoàn tiền theo yêu cầu #{request.Id}: {request.Reason}" + 
                    (refundFee > 0 ? $" (Phí: {refundFee:N0} VNĐ)" : "")
                );

                // ⭐ TRỪ TIỀN TỪ VÍ SHOP (toàn bộ số tiền, không trừ phí)
                // Phí hoàn tiền do shop giữ lại
                await _shopWalletService.RefundOrderPaymentAsync(shopId, request.OrderId, finalRefundAmount);

                // Cập nhật trạng thái request
                request.Status = "Approved";
                request.ApprovedAmount = refundAmount;
                request.ShopResponse = response;
                request.ProcessedByShopId = shopId;
                request.ProcessedAt = DateTime.UtcNow;
                request.UpdatedAt = DateTime.UtcNow;

                await _returnRequestRepository.UpdateAsync(request);

                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                return ServiceResult.Failure($"Lỗi khi hoàn tiền: {ex.Message}");
            }
        }

        public async Task<ServiceResult> RejectRequestAsync(Guid requestId, Guid shopId, string response)
        {
            var request = await _returnRequestRepository.GetByIdWithDetailsAsync(requestId);
            if (request == null)
            {
                return ServiceResult.Failure("Không tìm thấy yêu cầu hoàn trả.");
            }

            if (request.Status != "Pending")
            {
                return ServiceResult.Failure("Yêu cầu này đã được xử lý.");
            }

            // Verify shop ownership
            var belongsToShop = request.Order.OrderItems
                .Any(oi => oi.ProductVariant?.Product?.ShopId == shopId);

            if (!belongsToShop)
            {
                return ServiceResult.Failure("Bạn không có quyền xử lý yêu cầu này.");
            }

            if (string.IsNullOrWhiteSpace(response))
            {
                return ServiceResult.Failure("Vui lòng nhập lý do từ chối.");
            }

            request.Status = "Rejected";
            request.ShopResponse = response;
            request.ProcessedByShopId = shopId;
            request.ProcessedAt = DateTime.UtcNow;
            request.UpdatedAt = DateTime.UtcNow;

            await _returnRequestRepository.UpdateAsync(request);

            return ServiceResult.Success();
        }

        #endregion

        #region Mapping

        private ReturnRequestDto MapToDto(ReturnRequest request, Order order, Shop? shop)
        {
            var dto = new ReturnRequestDto
            {
                Id = request.Id,
                OrderId = request.OrderId,
                UserId = request.UserId,
                UserName = request.User?.Name ?? "N/A",
                UserEmail = request.User?.Email ?? "N/A",
                RequestType = request.RequestType,
                Reason = request.Reason,
                ReasonDetail = request.ReasonDetail,
                EvidenceImageUrls = !string.IsNullOrEmpty(request.EvidenceImages)
                    ? JsonSerializer.Deserialize<List<string>>(request.EvidenceImages) ?? new List<string>()
                    : new List<string>(),
                RequestedAmount = request.RequestedAmount,
                ApprovedAmount = request.ApprovedAmount,
                Status = request.Status,
                ShopResponse = request.ShopResponse,
                ProcessedByShopName = shop?.ShopName,
                ProcessedAt = request.ProcessedAt,
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt,
                OrderTotalAmount = order.TotalAmount,
                OrderStatus = order.Status,
                OrderDate = order.CreatedAt,
                OrderItems = order.OrderItems?.Select(oi => new ReturnRequestOrderItemDto
                {
                    ProductName = oi.ProductVariant?.Product?.Name ?? "N/A",
                    ImageUrl = oi.ProductVariant?.ImageUrl ?? oi.ProductVariant?.Product?.ImageUrl,
                    Price = oi.Price,
                    Quantity = oi.Quantity,
                    Size = oi.ProductVariant?.Size,
                    Color = oi.ProductVariant?.Color
                }).ToList() ?? new List<ReturnRequestOrderItemDto>()
            };

            return dto;
        }

        #endregion
    }
}
