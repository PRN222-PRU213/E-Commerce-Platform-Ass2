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

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 12;

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }

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

            // Lấy sản phẩm theo trang
            var productsResult = await _productService.GetByShopIdPagedAsync(
                shop.Id,
                CurrentPage,
                PageSize
            );
            var countsResult = await _productService.GetProductCountsByShopIdAsync(shop.Id);
            if (!productsResult.IsSuccess || !countsResult.IsSuccess)
            {
                TempData["ErrorMessage"] = productsResult.ErrorMessage ?? countsResult.ErrorMessage;
                return RedirectToPage("/Index");
            }

            var (products, totalCount) = productsResult.Data;
            var (total, active, inactive) = countsResult.Data;
            TotalCount = totalCount;
            TotalPages = (int)Math.Ceiling((double)TotalCount / PageSize);

            ViewModel = new ShopDashboardViewModel
            {
                Id = shop.Id,
                UserId = shop.UserId,
                ShopName = shop.ShopName,
                Description = shop.Description,
                Status = shop.Status,
                CreatedAt = shop.CreatedAt,
                Products = products.ToShopProductViewModels(),
                TotalProducts = total,
                ActiveProducts = active,
            };

            return Page();
        }
    }
}
