using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using E_Commerce_Platform_Ass2.Service.Services;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop.Products
{
    [Authorize]
    public class ReviewsModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;
        private readonly IShopService _shopService;

        public ReviewsModel(IProductService productService, IReviewService reviewService, IShopService shopService)
        {
            _productService = productService;
            _reviewService = reviewService;
            _shopService = shopService;
        }

        public ProductDetailDto Product { get; set; }
        public List<ReviewDto> Reviews { get; set; } = new();

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
            try
            {
                var (ok, shop) = await GetCurrentUserShopAsync();
                if (!ok || shop == null)
                {
                    return RedirectToPage("/Index");
                }

                var result = await _productService.GetProductDetailAsync(id, shop.Id);
                if (!result.IsSuccess || result.Data == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sản phẩm hoặc bạn không có quyền xem.";
                    return RedirectToPage("/Shop/Products/Index");
                }

                Product = result.Data;
                
                var reviews = await _reviewService.GetReviewsByProductIdAsync(id);
                if (reviews != null)
                {
                    Reviews = reviews.ToList();
                }

                return Page();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi tải dữ liệu bình luận.";
                return RedirectToPage("/Shop/ViewShop");
            }
        }

    }
}
