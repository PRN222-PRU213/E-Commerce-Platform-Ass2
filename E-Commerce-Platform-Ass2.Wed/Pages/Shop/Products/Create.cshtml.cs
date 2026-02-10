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
    public class CreateModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IShopService _shopService;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public CreateModel(
            IProductService productService,
            IShopService shopService,
            IHubContext<NotificationHub, INotificationClient> hubContext
        )
        {
            _productService = productService;
            _shopService = shopService;
            _hubContext = hubContext;
        }

        [BindProperty]
        public CreateProductViewModel Input { get; set; } = new();

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Guid.Empty;
            }

            return userId;
        }

        private async Task<bool> LoadCategoriesAsync()
        {
            var categories = await _productService.GetAllCategoriesAsync();
            Input.Categories = categories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();
            return true;
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
                TempData["ErrorMessage"] = "Bạn chưa có shop. Vui lòng đăng ký shop trước.";
                return RedirectToPage("/Index");
            }

            await LoadCategoriesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return RedirectToPage("/Authentication/Login");
            }

            var shop = await _shopService.GetShopByUserIdAsync(userId);
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Index");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return Page();
            }

            var dto = new CreateProductDto
            {
                ShopId = shop.Id,
                CategoryId = Input.CategoryId,
                Name = Input.Name,
                Description = Input.Description ?? string.Empty,
                BasePrice = Input.BasePrice,
                ImageUrl = Input.ImageUrl ?? string.Empty,
            };

            var result = await _productService.CreateProductAsync(dto);

            if (!result.IsSuccess)
            {
                ModelState.AddModelError(
                    string.Empty,
                    result.ErrorMessage ?? "Không thể tạo sản phẩm."
                );
                await LoadCategoriesAsync();
                return Page();
            }

            var message = new ProductChangedMessage
            {
                ProductId = result.Data,
                ShopId = shop.Id,
                ChangeType = "created",
                Status = "draft",
                Name = Input.Name,
                TriggeredBy = User.Identity?.Name,
            };

            await Task.WhenAll(
                _hubContext.Clients.Group($"shop-{shop.Id}").ProductChanged(message),
                _hubContext.Clients.Group("admins").ProductChanged(message),
                _hubContext.Clients.Group($"user-{userId}").ProductChanged(message)
            );

            TempData["SuccessMessage"] =
                "Tạo sản phẩm thành công! Vui lòng thêm biến thể cho sản phẩm.";
            return RedirectToPage("/Shop/Products/Edit", new { id = result.Data });
        }
    }
}
