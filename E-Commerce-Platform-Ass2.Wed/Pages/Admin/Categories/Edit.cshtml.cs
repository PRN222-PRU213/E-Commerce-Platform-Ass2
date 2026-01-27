using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Admin.Categories
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IAdminService _adminService;

        public EditModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [BindProperty]
        public EditCategoryViewModel Input { get; set; } = new();

        public int ProductCount { get; set; }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var result = await _adminService.GetCategoryByIdAsync(id);
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage;
                return RedirectToPage("/Admin/Categories/Index");
            }

            ProductCount = result.Data!.ProductCount;
            Input = result.Data.ToEditViewModel();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var dto = Input.ToDto();
            var result = await _adminService.UpdateCategoryAsync(id, dto);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Đã cập nhật danh mục thành công!";
                return RedirectToPage("/Admin/Categories/Index");
            }

            TempData["Error"] = result.ErrorMessage;
            return Page();
        }
    }
}

