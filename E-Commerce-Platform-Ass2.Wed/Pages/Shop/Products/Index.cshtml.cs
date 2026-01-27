using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop.Products
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IShopService _shopService;

        public IndexModel(IProductService productService, IShopService shopService)
        {
            _productService = productService;
            _shopService = shopService;
        }

        public ProductListViewModel ViewModel { get; set; } = new();

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Guid.Empty;
            }

            return userId;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToPage("/Authentication/Login");
            }

            var shop = await _shopService.GetShopByUserIdAsync(userId);
            if (shop == null)
            {
                TempData["ErrorMessage"] =
                    "Bạn chưa có shop. Vui lòng đăng ký shop trước khi quản lý sản phẩm.";
                return RedirectToPage("/Index");
            }

            var result = await _productService.GetByShopIdAsync(shop.Id);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToPage("/Index");
            }

            ViewModel = new ProductListViewModel
            {
                ShopId = shop.Id,
                ShopName = shop.ShopName,
                Products =
                    result
                        .Data?.Select(p => new ProductListItemViewModel
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description,
                            BasePrice = p.BasePrice,
                            Status = p.Status,
                            ImageUrl = p.ImageUrl,
                            CreatedAt = p.CreatedAt,
                            CategoryName = p.CategoryName,
                        })
                        .ToList() ?? new List<ProductListItemViewModel>(),
            };

            return Page();
        }
    }
}

