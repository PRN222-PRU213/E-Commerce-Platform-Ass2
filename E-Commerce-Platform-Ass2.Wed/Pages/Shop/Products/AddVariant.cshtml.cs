using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop.Products
{
    [Authorize]
    public class AddVariantModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariantService;
        private readonly IShopService _shopService;

        public AddVariantModel(
            IProductService productService,
            IProductVariantService productVariantService,
            IShopService shopService
        )
        {
            _productService = productService;
            _productVariantService = productVariantService;
            _shopService = shopService;
        }

        [BindProperty]
        public AddVariantViewModel Input { get; set; } = new();

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Guid.Empty;
            }

            return userId;
        }

        private async Task<(bool Ok, Data.Database.Entities.Shop? Shop)> GetCurrentUserShopAsync()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return (false, null);
            }

            var shop = await _shopService.GetShopByUserIdAsync(userId);
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return (false, null);
            }

            return (true, shop);
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var (ok, shop) = await GetCurrentUserShopAsync();
            if (!ok || shop == null)
            {
                return RedirectToPage("/Index");
            }

            var result = await _productService.GetProductDetailAsync(id, shop.Id);
            if (!result.IsSuccess || result.Data == null)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return RedirectToPage("/Shop/Products/Index");
            }

            var product = result.Data;
            if (product.Status != "draft")
            {
                TempData["ErrorMessage"] =
                    "Chỉ có thể thêm biến thể khi sản phẩm ở trạng thái bản nháp.";
                return RedirectToPage("/Shop/Products/Edit", new { id });
            }

            Input = new AddVariantViewModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(Guid id)
        {
            var (ok, shop) = await GetCurrentUserShopAsync();
            if (!ok || shop == null)
            {
                return RedirectToPage("/Index");
            }

            if (!ModelState.IsValid)
            {
                Input.ProductId = id;
                return Page();
            }

            var dto = new CreateProductVariantDto
            {
                ProductId = id,
                VariantName = Input.VariantName,
                Price = Input.Price,
                Size = Input.Size,
                Color = Input.Color,
                Stock = Input.Stock,
                Sku = Input.Sku ?? string.Empty,
                ImageUrl = Input.ImageUrl,
            };

            var result = await _productVariantService.AddVariantAsync(dto, shop.Id);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.ErrorMessage ?? "Không thể thêm biến thể."
                );
                Input.ProductId = id;
                return Page();
            }

            TempData["SuccessMessage"] = "Thêm biến thể thành công!";
            return RedirectToPage("/Shop/Products/Edit", new { id });
        }
    }
}

