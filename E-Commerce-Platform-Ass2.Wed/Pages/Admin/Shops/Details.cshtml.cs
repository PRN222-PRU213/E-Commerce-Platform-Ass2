using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Admin.Shops
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly IAdminService _adminService;

        public DetailsModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public AdminShopDetailViewModel Shop { get; set; } = new();

        private async Task<bool> LoadShopAsync(Guid id)
        {
            var result = await _adminService.GetShopDetailAsync(id);
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage;
                return false;
            }

            Shop = result.Data!.ToDetailViewModel();
            return true;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var ok = await LoadShopAsync(id);
            if (!ok)
            {
                return RedirectToPage("./Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostApproveShopAsync(Guid id)
        {
            var result = await _adminService.ApproveShopAsync(id);
            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "Shop đã được duyệt thành công!" : result.ErrorMessage;

            return RedirectToPage("./Details", new { id });
        }

        public async Task<IActionResult> OnPostRejectShopAsync(Guid id, string? reason)
        {
            var result = await _adminService.RejectShopAsync(id, reason);
            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "Shop đã bị từ chối!" : result.ErrorMessage;

            return RedirectToPage("./Details", new { id });
        }
    }
}

