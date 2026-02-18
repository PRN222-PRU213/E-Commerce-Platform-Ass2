using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Hubs;
using E_Commerce_Platform_Ass2.Wed.Models;
using E_Commerce_Platform_Ass2.Wed.Models.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Admin.Products
{
    [Authorize(Roles = "Admin")]
    public class PendingModel : PageModel
    {
        private readonly IAdminService _adminService;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public PendingModel(
            IAdminService adminService,
            IHubContext<NotificationHub, INotificationClient> hubContext
        )
        {
            _adminService = adminService;
            _hubContext = hubContext;
        }

        public List<AdminProductViewModel> Products { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var result = await _adminService.GetPendingProductsAsync();
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage;
                Products = new List<AdminProductViewModel>();
                return Page();
            }

            Products = result.Data!.ToViewModels();
            return Page();
        }

        public async Task<IActionResult> OnPostApproveProductAsync(Guid id)
        {
            // Get product info before approval for notification
            var productInfo = await _adminService.GetProductForApprovalAsync(id);

            var result = await _adminService.ApproveProductAsync(id);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
                ? "Sản phẩm đã được duyệt thành công!"
                : result.ErrorMessage;

            if (result.IsSuccess)
            {
                // 1. Send real-time notification to shop owner
                if (productInfo.IsSuccess && productInfo.Data != null)
                {
                    var notification = new NotificationMessage
                    {
                        Type = "success",
                        Message = $"Sản phẩm \"{productInfo.Data.Name}\" đã được Admin duyệt!",
                        Link = $"/Shop/Products/Edit?id={id}",
                        TimestampUtc = DateTime.UtcNow,
                    };
                    await _hubContext
                        .Clients.Group($"shop-{productInfo.Data.ShopId}")
                        .NotificationReceived(notification);
                }

                // 2. Broadcast to all customers to update homepage
                await BroadcastProductChangedAsync(
                    id,
                    "approved",
                    productInfo.Data?.Name,
                    productInfo.Data?.ShopId
                );
            }

            return RedirectToPage("/Admin/Products/Pending");
        }

        public async Task<IActionResult> OnPostRejectProductAsync(Guid id, string? reason)
        {
            // Get product info before rejection for notification
            var productInfo = await _adminService.GetProductForApprovalAsync(id);

            var result = await _adminService.RejectProductAsync(id, reason);
            TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
                ? "Sản phẩm đã bị từ chối!"
                : result.ErrorMessage;

            if (result.IsSuccess)
            {
                // 1. Send real-time notification to shop owner
                if (productInfo.IsSuccess && productInfo.Data != null)
                {
                    var msg = $"Sản phẩm \"{productInfo.Data.Name}\" đã bị từ chối";
                    if (!string.IsNullOrEmpty(reason))
                        msg += $": {reason}";

                    var notification = new NotificationMessage
                    {
                        Type = "error",
                        Message = msg,
                        Link = $"/Shop/Products/Edit?id={id}",
                        TimestampUtc = DateTime.UtcNow,
                    };
                    await _hubContext
                        .Clients.Group($"shop-{productInfo.Data.ShopId}")
                        .NotificationReceived(notification);
                }

                // 2. Broadcast to all customers (optional, but good for consistency)
                await BroadcastProductChangedAsync(
                    id,
                    "rejected",
                    productInfo.Data?.Name,
                    productInfo.Data?.ShopId
                );
            }

            return RedirectToPage("/Admin/Products/Pending");
        }

        private async Task BroadcastProductChangedAsync(
            Guid productId,
            string changeType,
            string? name = null,
            Guid? shopId = null
        )
        {
            var message = new ProductChangedMessage
            {
                ProductId = productId,
                ShopId = shopId ?? Guid.Empty,
                ChangeType = changeType,
                Status = changeType == "approved" ? "active" : "rejected",
                Name = name,
                TriggeredBy = "admin",
            };
            await _hubContext.Clients.All.ProductChanged(message);
        }
    }
}
