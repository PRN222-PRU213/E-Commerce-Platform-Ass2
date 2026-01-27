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
    public class ViewShopModel : PageModel
    {
        private readonly IShopService _shopService;
        private readonly IProductService _productService;

        public ViewShopModel(IShopService shopService, IProductService productService)
        {
            _shopService = shopService;
            _productService = productService;
        }

        public ShopDashboardViewModel ViewModel { get; set; } = new();

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

            // Lấy toàn bộ sản phẩm của shop (tất cả status vì đây là trang quản lý)
            var productsResult = await _productService.GetByShopIdAsync(shop.Id);
            var products =
                productsResult.IsSuccess && productsResult.Data != null
                    ? productsResult.Data.ToList()
                    : new List<E_Commerce_Platform_Ass2.Service.DTOs.ProductDto>();

            ViewModel = new ShopDashboardViewModel
            {
                Id = shop.Id,
                UserId = shop.UserId,
                ShopName = shop.ShopName,
                Description = shop.Description,
                Status = shop.Status,
                CreatedAt = shop.CreatedAt,
                Products = products.ToShopProductViewModels(),
            };

            return Page();
        }
    }
}
