using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Infrastructure.Extensions;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop
{
    [AllowAnonymous]
    public class DetailModel : PageModel
    {
        private readonly IShopService _shopService;
        private readonly IProductService _productService;

        public DetailModel(IShopService shopService, IProductService productService)
        {
            _shopService = shopService;
            _productService = productService;
        }

        public ShopDetailViewModel ViewModel { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var shop = await _shopService.GetShopDtoByIdAsync(id);
            if (shop == null)
            {
                return NotFound();
            }

            // Chỉ hiển thị shop đã được duyệt (Active)
            if (shop.Status != "Active")
            {
                return NotFound();
            }

            // Lấy danh sách sản phẩm của shop (chỉ sản phẩm đã được duyệt)
            var productsResult = await _productService.GetByShopIdAsync(id);
            var products =
                productsResult.IsSuccess && productsResult.Data != null
                    ? productsResult.Data.Where(p => p.Status == "active").ToList()
                    : new List<ProductDto>();

            ViewModel = new ShopDetailViewModel
            {
                Id = shop.Id,
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

