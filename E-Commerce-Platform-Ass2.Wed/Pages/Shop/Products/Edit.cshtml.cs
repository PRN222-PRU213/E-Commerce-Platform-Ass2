using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Hubs;
using E_Commerce_Platform_Ass2.Wed.Models;
using E_Commerce_Platform_Ass2.Wed.Models.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop.Products
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IProductVariantService _productVariantService;
        private readonly IShopService _shopService;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public EditModel(
            IProductService productService,
            IProductVariantService productVariantService,
            IShopService shopService,
            IHubContext<NotificationHub, INotificationClient> hubContext
        )
        {
            _productService = productService;
            _productVariantService = productVariantService;
            _shopService = shopService;
            _hubContext = hubContext;
        }

        [BindProperty]
        public EditProductViewModel Input { get; set; } = new();

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

        private static EditProductViewModel MapToEditViewModel(
            ProductDetailDto product,
            IEnumerable<CategoryDto> categories
        )
        {
            return new EditProductViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                BasePrice = product.BasePrice,
                Status = product.Status,
                ImageUrl = product.ImageUrl,
                CategoryName = product.CategoryName,
                CategoryId = product.CategoryId,
                CreatedAt = product.CreatedAt,
                Categories = categories
                    .Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Name,
                        Selected = c.Id == product.CategoryId,
                    })
                    .ToList(),
                Variants = product
                    .Variants.Select(v => new ProductVariantViewModel
                    {
                        Id = v.Id,
                        VariantName = v.VariantName,
                        Price = v.Price,
                        Size = v.Size,
                        Color = v.Color,
                        Stock = v.Stock,
                        Sku = v.Sku,
                        Status = v.Status,
                        ImageUrl = v.ImageUrl,
                    })
                    .ToList(),
            };
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
                TempData["ErrorMessage"] = result.ErrorMessage ?? "Không tìm thấy sản phẩm.";
                return RedirectToPage("/Shop/Products/Index");
            }

            var categories = await _productService.GetAllCategoriesAsync();
            Input = MapToEditViewModel(result.Data, categories);
            return Page();
        }

        private async Task LoadCategoriesAndVariantsAsync(Guid productId, Guid shopId)
        {
            var categories = await _productService.GetAllCategoriesAsync();
            Input.Categories = categories
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Id == Input.CategoryId,
                })
                .ToList();

            var detailResult = await _productService.GetProductDetailAsync(productId, shopId);
            if (detailResult.IsSuccess && detailResult.Data != null)
            {
                Input.Variants = detailResult
                    .Data.Variants.Select(v => new ProductVariantViewModel
                    {
                        Id = v.Id,
                        VariantName = v.VariantName,
                        Price = v.Price,
                        Size = v.Size,
                        Color = v.Color,
                        Stock = v.Stock,
                        Sku = v.Sku,
                        Status = v.Status,
                        ImageUrl = v.ImageUrl,
                    })
                    .ToList();
            }
        }

        private Task NotifyProductChangedAsync(
            Guid productId,
            Guid shopId,
            string changeType,
            string? status = null,
            string? name = null
        )
        {
            var message = new ProductChangedMessage
            {
                ProductId = productId,
                ShopId = shopId,
                ChangeType = changeType,
                Status = status,
                Name = name ?? Input.Name,
                TriggeredBy = User.Identity?.Name,
            };

            return Task.WhenAll(
                _hubContext.Clients.Group($"shop-{shopId}").ProductChanged(message),
                _hubContext.Clients.Group("admins").ProductChanged(message),
                _hubContext.Clients.Group($"user-{GetCurrentUserId()}").ProductChanged(message)
            );
        }

        public async Task<IActionResult> OnPostUpdateAsync(Guid id)
        {
            var (ok, shop) = await GetCurrentUserShopAsync();
            if (!ok || shop == null)
            {
                return RedirectToPage("/Index");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAndVariantsAsync(id, shop.Id);
                return Page();
            }

            var dto = new UpdateProductDto
            {
                ProductId = id,
                ShopId = shop.Id,
                CategoryId = Input.CategoryId,
                Name = Input.Name,
                Description = Input.Description ?? string.Empty,
                BasePrice = Input.BasePrice,
                ImageUrl = Input.ImageUrl ?? string.Empty,
            };

            var result = await _productService.UpdateProductAsync(dto);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                await LoadCategoriesAndVariantsAsync(id, shop.Id);
                return Page();
            }

            await NotifyProductChangedAsync(id, shop.Id, "updated", Input.Status, Input.Name);

            TempData["SuccessMessage"] = "Cập nhật thông tin sản phẩm thành công!";
            return RedirectToPage("/Shop/Products/Edit", new { id });
        }

        public async Task<IActionResult> OnPostSubmitAsync(Guid id)
        {
            var (ok, shop) = await GetCurrentUserShopAsync();
            if (!ok || shop == null)
            {
                return RedirectToPage("/Index");
            }

            var result = await _productService.SubmitProductAsync(id, shop.Id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }
            else
            {
                TempData["SuccessMessage"] =
                    "Đã gửi sản phẩm để duyệt thành công! Vui lòng chờ admin phê duyệt.";

                // Fetch full product details to include in the broadcast
                var productDetail = await _productService.GetProductDetailDtoAsync(id);

                var message = new ProductChangedMessage
                {
                    ProductId = id,
                    ShopId = shop.Id,
                    ChangeType = "statusChanged",
                    Status = "pending",
                    Name = productDetail?.Name ?? Input.Name,
                    TriggeredBy = User.Identity?.Name,
                    ImageUrl = productDetail?.ImageUrl,
                    BasePrice = productDetail?.BasePrice ?? 0,
                    CategoryName = productDetail?.CategoryName,
                    ShopName = productDetail?.ShopName ?? shop.ShopName,
                    Description = productDetail?.Description
                };

                await Task.WhenAll(
                    _hubContext.Clients.Group($"shop-{shop.Id}").ProductChanged(message),
                    _hubContext.Clients.Group("admins").ProductChanged(message),
                    _hubContext.Clients.Group($"user-{GetCurrentUserId()}").ProductChanged(message)
                );
            }

            return RedirectToPage("/Shop/Products/Edit", new { id });
        }

        public async Task<IActionResult> OnPostUnpublishAsync(Guid id)
        {
            var (ok, shop) = await GetCurrentUserShopAsync();
            if (!ok || shop == null)
            {
                return RedirectToPage("/Index");
            }

            var result = await _productService.UnpublishProductAsync(id, shop.Id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }
            else
            {
                TempData["SuccessMessage"] =
                    "Đã gỡ sản phẩm thành công! Bạn có thể chỉnh sửa và gửi duyệt lại.";
                await NotifyProductChangedAsync(id, shop.Id, "statusChanged", "draft", Input.Name);
            }

            return RedirectToPage("/Shop/Products/Edit", new { id });
        }

        public async Task<IActionResult> OnPostDeleteVariantAsync(Guid variantId, Guid productId)
        {
            var (ok, shop) = await GetCurrentUserShopAsync();
            if (!ok || shop == null)
            {
                return RedirectToPage("/Index");
            }

            var result = await _productVariantService.DeleteVariantAsync(variantId, shop.Id);

            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }
            else
            {
                TempData["SuccessMessage"] = "Xóa biến thể thành công!";
            }

            return RedirectToPage("/Shop/Products/Edit", new { id = productId });
        }
    }
}
