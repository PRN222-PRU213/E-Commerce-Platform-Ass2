using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Admin.Products
{
    [Authorize(Roles = "Admin")]
    public class PendingModel : PageModel
    {
        private readonly IAdminService _adminService;

        public PendingModel(IAdminService adminService)
        {
            _adminService = adminService;
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

