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
        private readonly IWalletService _walletService;
        private readonly IShopWalletRepository _shopWalletRepository;
        private readonly IShopRepository _shopRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly RefundBusinessRules _rules;

        public ReturnRequestService(
            IReturnRequestRepository returnRequestRepository,
            IOrderRepository orderRepository,
            IRefundService refundService,
            IShopWalletService shopWalletService,
            IWalletService walletService,
            IShopWalletRepository shopWalletRepository,
            IShopRepository shopRepository,
            IWalletRepository walletRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IOptions<RefundBusinessRules> rulesOptions)
        {
            _returnRequestRepository = returnRequestRepository;
            _orderRepository = orderRepository;
            _refundService = refundService;
            _shopWalletService = shopWalletService;
            _walletService = walletService;
            _shopWalletRepository = shopWalletRepository;
            _shopRepository = shopRepository;
            _walletRepository = walletRepository;
            _walletTransactionRepository = walletTransactionRepository;
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

            // Chỉ cho phép khi khách đã xác nhận nhận hàng
            if (!string.Equals(order.Status, "Delivered", StringComparison.OrdinalIgnoreCase))
            {
                return ServiceResult<ReturnRequestDto>.Failure("Chỉ có thể yêu cầu hoàn hàng sau khi bạn đã nhận hàng.");
            }

            // ⭐ BUSINESS RULE: Kiểm tra thời hạn hoàn hàng trong 7 ngày kể từ thời điểm nhận hàng
            var receivedAt = order.CompletedAt ?? order.CreatedAt;
            var daysSinceReceived = (DateTime.UtcNow - receivedAt).TotalDays;
            if (daysSinceReceived > 7)
            {
                return ServiceResult<ReturnRequestDto>.Failure(
                    "Đã quá thời hạn 7 ngày kể từ khi nhận hàng. Bạn không thể gửi yêu cầu hoàn hàng nữa.");
            }

            if (string.IsNullOrWhiteSpace(dto.Reason))
            {
                return ServiceResult<ReturnRequestDto>.Failure("Vui lòng nhập lý do hoàn hàng.");
            }

            if (dto.EvidenceImageUrls == null || !dto.EvidenceImageUrls.Any())
            {
                return ServiceResult<ReturnRequestDto>.Failure("Vui lòng cung cấp ít nhất 1 hình ảnh bằng chứng để shop xác nhận.");
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

            if (!string.Equals(order.Status, "Delivered", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var receivedAt = order.CompletedAt ?? order.CreatedAt;
            var daysSinceReceived = (DateTime.UtcNow - receivedAt).TotalDays;
            if (daysSinceReceived > 7)
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

            // ⭐ BUSINESS RULE THỰC TẾ:
            // - Nếu lỗi shop: khách nhận đủ 100%, shop trả thêm chiết khấu nền tảng
            // - Nếu lỗi khách: khách nhận tiền sau khi trừ phí, không tính chiết khấu shop
            var isShopFault = _rules.IsShopFault(request.Reason);
            var baseAmount = request.Order.TotalAmount;
            var refundFee = _rules.CalculateFee(baseAmount, request.Reason);
            var customerRefundAmount = isShopFault ? baseAmount : Math.Max(0, baseAmount - refundFee);
            var commissionAmount = isShopFault
                ? customerRefundAmount * _rules.ShopRefundCommissionPercent / 100
                : 0;
            var totalShopDeduction = customerRefundAmount + commissionAmount;
            var alreadyRefunded = await _refundService.IsRefundedAsync(request.OrderId);

            var shop = await _shopRepository.GetByIdAsync(shopId);
            if (shop == null)
            {
                return ServiceResult.Failure("Không tìm thấy thông tin shop.");
            }

            var ownerWallet = await _walletRepository.GetByUserIdAsync(shop.UserId);
            if (ownerWallet == null)
            {
                ownerWallet = new Wallet
                {
                    WalletId = Guid.NewGuid(),
                    UserId = shop.UserId,
                    Balance = 0,
                    LastChangeAmount = 0,
                    LastChangeType = "Init",
                    UpdatedAt = DateTime.UtcNow
                };
                await _walletRepository.AddAsync(ownerWallet);
            }

            // Chặn sớm trường hợp tổng số dư không đủ để tránh hoàn tiền trước rồi mới thất bại ở bước trừ ví
            var shopWallet = await _shopWalletRepository.GetOrCreateAsync(shopId);
            var combinedBalance = ownerWallet.Balance + shopWallet.Balance;
            if (combinedBalance < totalShopDeduction)
            {
                return ServiceResult.Failure(
                    $"Số dư ví không đủ để hoàn tiền và chiết khấu. " +
                    $"Ví shop owner: {ownerWallet.Balance:N0} đ, ví shop hệ thống: {shopWallet.Balance:N0} đ, cần: {totalShopDeduction:N0} đ."
                );
            }

            try
            {
                // Nếu đơn chưa hoàn tiền thì thực hiện hoàn tiền; nếu đã hoàn rồi thì bỏ qua để tránh gọi hoàn tiền lần 2
                if (!alreadyRefunded)
                {
                    await _refundService.RefundAsync(
                        request.OrderId,
                        customerRefundAmount,
                        $"Hoàn tiền theo yêu cầu #{request.Id}: {request.Reason}" +
                        (!isShopFault && refundFee > 0 ? $" (Phí: {refundFee:N0} VNĐ)" : "")
                    );
                }

                // ⭐ TRỪ TIỀN TỪ VÍ SHOP OWNER trước, thiếu thì trừ tiếp từ ví shop hệ thống
                var remainingDeduction = totalShopDeduction;

                if (ownerWallet.Balance > 0)
                {
                    var deductFromOwnerWallet = Math.Min(ownerWallet.Balance, remainingDeduction);
                    ownerWallet.Balance -= deductFromOwnerWallet;
                    ownerWallet.LastChangeAmount = -deductFromOwnerWallet;
                    ownerWallet.LastChangeType = "ShopRefund";
                    ownerWallet.UpdatedAt = DateTime.UtcNow;
                    await _walletRepository.UpdateAsync(ownerWallet);

                    await _walletTransactionRepository.AddAsync(new WalletTransaction
                    {
                        Id = Guid.NewGuid(),
                        WalletId = ownerWallet.WalletId,
                        TransactionType = "ShopRefund",
                        Amount = -deductFromOwnerWallet,
                        BalanceAfter = ownerWallet.Balance,
                        Description = $"Trừ ví shop owner để hoàn tiền đơn #{request.OrderId.ToString()[..8].ToUpper()}",
                        ReferenceId = request.OrderId.ToString(),
                        CreatedAt = DateTime.UtcNow
                    });

                    remainingDeduction -= deductFromOwnerWallet;
                }

                if (remainingDeduction > 0)
                {
                    var shopRefundResult = await _shopWalletService.RefundOrderPaymentAsync(shopId, request.OrderId, remainingDeduction);
                    if (!shopRefundResult.IsSuccess)
                    {
                        throw new Exception(shopRefundResult.ErrorMessage ?? "Không thể trừ tiền từ ví shop.");
                    }
                }

                if (commissionAmount > 0)
                {
                    await _walletService.CreditAdminCommissionAsync(
                        request.OrderId,
                        commissionAmount,
                        $"Chiết khấu {_rules.ShopRefundCommissionPercent}% từ hoàn tiền đơn #{request.OrderId.ToString()[..8].ToUpper()}"
                    );
                }

                // Cập nhật trạng thái request
                request.Status = "Approved";
                request.ApprovedAmount = customerRefundAmount;
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
