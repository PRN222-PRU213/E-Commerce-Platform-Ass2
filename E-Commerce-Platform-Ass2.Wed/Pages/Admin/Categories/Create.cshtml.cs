using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Admin.Categories
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly IAdminService _adminService;

        public CreateModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [BindProperty]
        public CreateCategoryViewModel Input { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var dto = Input.ToDto();
            var result = await _adminService.CreateCategoryAsync(dto);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Đã tạo danh mục thành công!";
                return RedirectToPage("/Admin/Categories/Index");
            }

            TempData["Error"] = result.ErrorMessage;
            return Page();
        }
    }
}

