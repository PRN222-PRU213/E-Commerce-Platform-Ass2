using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Hubs;
using E_Commerce_Platform_Ass2.Wed.Models.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop.Orders
{
    [Authorize]
    public class ReturnRequestDetailModel : PageModel
    {
        private readonly IShopService _shopService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public ReturnRequestDetailModel(IShopService shopService,
            IReturnRequestService returnRequestService,
            IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _shopService = shopService;
            _returnRequestService = returnRequestService;
            _hubContext = hubContext;
        }

        public ReturnRequestDto ReturnReq { get; set; } = new();

        [BindProperty]
        public decimal? ApprovedAmount { get; set; }

        [BindProperty]
        public string? ShopResponse { get; set; }

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

            ReturnReq = await _returnRequestService.GetShopRequestDetailAsync(id, shopId.Value);

            if (ReturnReq == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy yêu cầu này.";
                return RedirectToPage("./ReturnRequests");
            }

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

            var result = await _returnRequestService.ApproveRequestAsync(id, shopId.Value, ApprovedAmount, ShopResponse);

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
