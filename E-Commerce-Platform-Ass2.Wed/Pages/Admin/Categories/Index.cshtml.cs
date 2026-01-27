using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Admin.Categories
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IAdminService _adminService;

        public IndexModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public List<AdminCategoryViewModel> Categories { get; set; } = new();

        public async Task OnGetAsync()
        {
            var result = await _adminService.GetAllCategoriesAsync();
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage;
                Categories = new List<AdminCategoryViewModel>();
                return;
            }

            Categories = result.Data!.ToViewModels();
        }
    }
}

