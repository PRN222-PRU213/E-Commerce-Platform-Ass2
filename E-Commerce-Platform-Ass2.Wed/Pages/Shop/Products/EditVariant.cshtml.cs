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
    public class EditVariantModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariantService;
        private readonly IShopService _shopService;

        public EditVariantModel(
            IProductService productService,
            IProductVariantService productVariantService,
            IShopService shopService)
        {
            _productService = productService;
            _productVariantService = productVariantService;
            _shopService = shopService;
        }

        [BindProperty]
        public EditVariantViewModel Input { get; set; } = new();

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Guid.Empty;
            }
            return userId;
        }

        private async Task<(bool Ok, E_Commerce_Platform_Ass2.Data.Database.Entities.Shop? Shop)> GetCurrentUserShopAsync()
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

        public async Task<IActionResult> OnGetAsync(Guid variantId)
        {
            var (ok, shop) = await GetCurrentUserShopAsync();
            if (!ok || shop == null)
            {
                return RedirectToPage("/Index");
            }

            // Get variant details
            var variantResult = await _productVariantService.GetByIdAsync(variantId);
            if (!variantResult.IsSuccess || variantResult.Data == null)
            {
                TempData["ErrorMessage"] = variantResult.ErrorMessage ?? "Không tìm thấy biến thể.";
                return RedirectToPage("/Shop/Products/Index");
            }

            var variant = variantResult.Data;

            // Get product to check ownership and status
            var productResult = await _productService.GetProductDetailAsync(variant.ProductId, shop.Id);
            if (!productResult.IsSuccess)
            {
                TempData["ErrorMessage"] = productResult.ErrorMessage ?? "Không tìm thấy sản phẩm.";
                return RedirectToPage("/Shop/Products/Index");
            }

            var product = productResult.Data!;
            if (product.Status != "draft")
            {
                TempData["ErrorMessage"] = "Chỉ có thể chỉnh sửa biến thể khi sản phẩm ở trạng thái bản nháp.";
                return RedirectToPage("/Shop/Products/Edit", new { id = variant.ProductId });
            }

            Input = new EditVariantViewModel
            {
                Id = variant.Id,
                ProductId = variant.ProductId,
                ProductName = product.Name,
                VariantName = variant.VariantName,
                Price = variant.Price,
                Size = variant.Size,
                Color = variant.Color,
                Stock = variant.Stock,
                Sku = variant.Sku,
                ImageUrl = variant.ImageUrl,
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var (ok, shop) = await GetCurrentUserShopAsync();
            if (!ok || shop == null)
            {
                return RedirectToPage("/Index");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }

            var dto = new UpdateProductVariantDto
            {
                Id = Input.Id,
                VariantName = Input.VariantName,
                Price = Input.Price,
                Size = Input.Size,
                Color = Input.Color,
                Stock = Input.Stock,
                Sku = Input.Sku ?? string.Empty,
                ImageUrl = Input.ImageUrl,
            };

            var result = await _productVariantService.UpdateVariantAsync(dto, shop.Id);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.ErrorMessage ?? "Không thể cập nhật biến thể."
                );
                return Page();
            }

            TempData["SuccessMessage"] = "Cập nhật biến thể thành công!";
            return RedirectToPage("/Shop/Products/Edit", new { id = Input.ProductId });
        }
    }
}
