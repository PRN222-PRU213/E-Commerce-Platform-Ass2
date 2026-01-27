using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Admin.Categories
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly IAdminService _adminService;

        public DeleteModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            var result = await _adminService.DeleteCategoryAsync(id);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Đã xóa danh mục thành công!";
            }
            else
            {
                TempData["Error"] = result.ErrorMessage;
            }

            return RedirectToPage("/Admin/Categories/Index");
        }
    }
}

