using System.Security.Claims;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Options;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Hubs;
using E_Commerce_Platform_Ass2.Wed.Models.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop.Orders
{
    [Authorize]
    public class ReturnRequestDetailModel : PageModel
    {
        private readonly IShopService _shopService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly RefundBusinessRules _refundRules;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public ReturnRequestDetailModel(IShopService shopService,
            IReturnRequestService returnRequestService,
            INotificationService notificationService,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IOptions<RefundBusinessRules> refundRulesOptions,
            IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _shopService = shopService;
            _returnRequestService = returnRequestService;
            _notificationService = notificationService;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _refundRules = refundRulesOptions.Value;
            _hubContext = hubContext;
        }

        public ReturnRequestDto ReturnReq { get; set; } = new();

        [BindProperty]
        public decimal? ApprovedAmount { get; set; }

        [BindProperty]
        public string? RejectReason { get; set; }

        private async Task<Guid?> GetCurrentShopIdAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return null;
            }

            var shop = await _shopService.GetShopByUserIdAsync(userId);
            return shop?.Id;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Shop/RegisterShop");
            }

            var requestDetail = await _returnRequestService.GetShopRequestDetailAsync(id, shopId.Value);

            if (requestDetail == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy yêu cầu này.";
                return RedirectToPage("./ReturnRequests");
            }

            ReturnReq = requestDetail;

            return Page();
        }

        public async Task<IActionResult> OnPostApproveReturnAsync(Guid id)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("./ReturnRequests");
            }

            // Get request detail for notification
            var requestDetail = await _returnRequestService.GetShopRequestDetailAsync(id, shopId.Value);

            var result = await _returnRequestService.ApproveRequestAsync(id, shopId.Value, ApprovedAmount, null);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Đã duyệt yêu cầu và hoàn tiền thành công!";

                // Notify customer about approved return
                if (requestDetail != null)
                {
                    var notification = new NotificationMessage
                    {
                        Type = "success",
                        Message = $"Yêu cầu hoàn trả cho đơn hàng #{requestDetail.OrderId.ToString()[..8].ToUpper()} đã được duyệt!",
                        Link = "/ReturnRequest/Index",
                        UserId = requestDetail.UserId
                    };
                    await _hubContext.Clients.Group($"user-{requestDetail.UserId}")
                        .NotificationReceived(notification);

                    // Notify admins if this approval includes platform commission
                    if (_refundRules.IsShopFault(requestDetail.Reason) && _refundRules.ShopRefundCommissionPercent > 0)
                    {
                        var commissionAmount = requestDetail.OrderTotalAmount * _refundRules.ShopRefundCommissionPercent / 100;

                        if (commissionAmount > 0)
                        {
                            var adminRole = await _roleRepository.GetByNameAsync("Admin");
                            if (adminRole != null)
                            {
                                var users = await _userRepository.GetAllAsync();
                                var adminUsers = users.Where(u => u.RoleId == adminRole.RoleId).ToList();

                                var adminMessage =
                                    $"Đã nhận chiết khấu {commissionAmount:N0} đ từ hoàn hàng đơn #{requestDetail.OrderId.ToString()[..8].ToUpper()}";
                                var adminLink = "/Authentication/Profile";

                                foreach (var admin in adminUsers)
                                {
                                    await _notificationService.CreateNotificationAsync(admin.Id, "success", adminMessage, adminLink);
                                }

                                await _hubContext.Clients.Group("admins").NotificationReceived(new NotificationMessage
                                {
                                    Type = "success",
                                    Message = adminMessage,
                                    Link = adminLink,
                                    UserId = requestDetail.UserId
                                });
                            }
                        }
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }

            return RedirectToPage("./ReturnRequestDetail", new { id });
        }

        public async Task<IActionResult> OnPostRejectReturnAsync(Guid id)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("./ReturnRequests");
            }

            // Get request detail for notification
            var requestDetail = await _returnRequestService.GetShopRequestDetailAsync(id, shopId.Value);

            var result = await _returnRequestService.RejectRequestAsync(id, shopId.Value, RejectReason ?? string.Empty);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Đã từ chối yêu cầu hoàn trả.";

                // Notify customer about rejected return
                if (requestDetail != null)
                {
                    var msg = $"Yêu cầu hoàn trả cho đơn hàng #{requestDetail.OrderId.ToString()[..8].ToUpper()} đã bị từ chối";
                    if (!string.IsNullOrEmpty(RejectReason)) msg += $": {RejectReason}";

                    var notification = new NotificationMessage
                    {
                        Type = "error",
                        Message = msg,
                        Link = "/ReturnRequest/Index",
                        UserId = requestDetail.UserId
                    };
                    await _hubContext.Clients.Group($"user-{requestDetail.UserId}")
                        .NotificationReceived(notification);
                }
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }

            return RedirectToPage("./ReturnRequestDetail", new { id });
        }
    }
}
