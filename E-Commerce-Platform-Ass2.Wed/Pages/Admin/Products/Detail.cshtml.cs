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
    public class DetailModel : PageModel
    {
        private readonly IAdminService _adminService;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public DetailModel(IAdminService adminService,
            IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _adminService = adminService;
            _hubContext = hubContext;
        }

        public AdminProductDetailViewModel Product { get; set; } = new();

        private async Task<bool> LoadProductAsync(Guid id)
        {
            var result = await _adminService.GetProductForApprovalAsync(id);
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage;
                return false;
            }

            Product = result.Data!.ToDetailViewModel();
            return true;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var ok = await LoadProductAsync(id);
            if (!ok)
            {
                return RedirectToPage("/Admin/Products/Pending");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostApproveProductAsync(Guid id)
        {
            var result = await _adminService.ApproveProductAsync(id);
            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "Sản phẩm đã được duyệt thành công!" : result.ErrorMessage;

            if (result.IsSuccess)
            {
                await BroadcastProductChangedAsync(id, "approved");
            }

            return RedirectToPage("/Admin/Products/Pending");
        }

        public async Task<IActionResult> OnPostRejectProductAsync(Guid id, string? reason)
        {
            var result = await _adminService.RejectProductAsync(id, reason);
            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "Sản phẩm đã bị từ chối!" : result.ErrorMessage;

            if (result.IsSuccess)
            {
                await BroadcastProductChangedAsync(id, "rejected");
            }

            return RedirectToPage("/Admin/Products/Pending");
        }

        private async Task BroadcastProductChangedAsync(Guid productId, string changeType)
        {
            var message = new ProductChangedMessage
            {
                ProductId = productId,
                ChangeType = changeType,
                Status = changeType == "approved" ? "active" : "rejected",
                TriggeredBy = "admin"
            };
            await _hubContext.Clients.All.ProductChanged(message);
        }
    }
}
