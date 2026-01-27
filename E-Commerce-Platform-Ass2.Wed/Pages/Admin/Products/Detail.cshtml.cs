using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Admin.Products
{
    [Authorize(Roles = "Admin")]
    public class DetailModel : PageModel
    {
        private readonly IAdminService _adminService;

        public DetailModel(IAdminService adminService)
        {
            _adminService = adminService;
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

            return RedirectToPage("/Admin/Products/Pending");
        }

        public async Task<IActionResult> OnPostRejectProductAsync(Guid id, string? reason)
        {
            var result = await _adminService.RejectProductAsync(id, reason);
            TempData[result.IsSuccess ? "Success" : "Error"] =
                result.IsSuccess ? "Sản phẩm đã bị từ chối!" : result.ErrorMessage;

            return RedirectToPage("/Admin/Products/Pending");
        }
    }
}

