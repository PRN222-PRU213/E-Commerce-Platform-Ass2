using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Admin.Shops
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IAdminService _adminService;

        public IndexModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public List<AdminShopViewModel> Shops { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var result = string.IsNullOrEmpty(Status)
                ? await _adminService.GetAllShopsAsync()
                : await _adminService.GetShopsByStatusAsync(Status);

            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage;
                Shops = new List<AdminShopViewModel>();
                return Page();
            }

            Shops = result.Data!.ToViewModels();
            ViewData["CurrentStatus"] = Status;
            return Page();
        }
    }
}

