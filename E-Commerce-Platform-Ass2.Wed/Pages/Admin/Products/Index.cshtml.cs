using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Admin.Products
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IAdminService _adminService;

        public IndexModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public List<AdminProductViewModel> Products { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var result = string.IsNullOrEmpty(Status)
                ? await _adminService.GetAllProductsAsync()
                : await _adminService.GetProductsByStatusAsync(Status);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage;
                Products = new List<AdminProductViewModel>();
                return Page();
            }

            Products = result.Data!.ToViewModels();
            ViewData["CurrentStatus"] = Status;
            return Page();
        }
    }
}

