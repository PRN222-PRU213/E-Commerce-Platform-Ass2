using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop
{
    [Authorize]
    public class RegisterShopModel : PageModel
    {
        private readonly IShopService _shopService;

        public RegisterShopModel(IShopService shopService)
        {
            _shopService = shopService;
        }

        [BindProperty]
        public RegisterShopViewModel Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // Kiểm tra xem user đã có shop chưa
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToPage("/Authentication/Login");
            }

            var hasShop = await _shopService.UserHasShopAsync(userId);
            if (hasShop)
            {
                TempData["ErrorMessage"] =
                    "Bạn đã có shop rồi. Mỗi tài khoản chỉ được đăng ký một shop.";
                return RedirectToPage("/Authentication/Profile");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToPage("/Authentication/Login");
            }

            // Kiểm tra xem user đã có shop chưa
            var hasShop = await _shopService.UserHasShopAsync(userId);
            if (hasShop)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Bạn đã có shop rồi. Mỗi tài khoản chỉ được đăng ký một shop."
                );
                return Page();
            }

            // Kiểm tra tên shop đã tồn tại chưa
            var shopNameExists = await _shopService.ShopNameExistsAsync(Input.ShopName.Trim());
            if (shopNameExists)
            {
                ModelState.AddModelError(
                    nameof(Input.ShopName),
                    "Tên shop này đã được sử dụng. Vui lòng chọn tên khác."
                );
                return Page();
            }

            // Tạo shop mới thông qua service layer
            await _shopService.RegisterShopAsync(userId, Input.ShopName, Input.Description);

            TempData["SuccessMessage"] =
                "Đăng ký shop thành công! Shop của bạn đang chờ phê duyệt.";
            return RedirectToPage("/Authentication/Profile");
        }
    }
}
