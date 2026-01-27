using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Infrastructure.Extensions;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop
{
    [Authorize]
    public class StatisticsModel : PageModel
    {
        private readonly IShopService _shopService;

        public StatisticsModel(IShopService shopService)
        {
            _shopService = shopService;
        }

        public ShopStatisticsViewModel ViewModel { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToPage("/Authentication/Login");
            }

            var shop = await _shopService.GetShopDtoByUserIdAsync(userId);
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop. Vui lòng đăng ký shop trước.";
                return RedirectToPage("/Shop/RegisterShop");
            }

            // Kiểm tra shop đã được duyệt chưa
            if (shop.Status != "Active")
            {
                TempData["ErrorMessage"] =
                    "Shop của bạn chưa được duyệt. Vui lòng chờ admin phê duyệt.";
                return RedirectToPage("/Shop/ViewShop");
            }

            var statistics = await _shopService.GetShopStatisticsAsync(shop.Id);
            ViewModel = statistics.ToViewModel();
            return Page();
        }
    }
}

