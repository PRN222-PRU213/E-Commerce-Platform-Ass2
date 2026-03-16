using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Product
{
    public class DetailModel : PageModel
    {
        private readonly IProductService _productService;
        private readonly IReviewService _reviewService;
        private readonly IPreOrderService _preOrderService;

        public DetailModel(
            IProductService productService,
            IReviewService reviewService,
            IPreOrderService preOrderService
        )
        {
            _productService = productService;
            _reviewService = reviewService;
            _preOrderService = preOrderService;
        }

        public ProductDetailViewModel ViewModel { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var productDto = await _productService.GetProductDetailDtoAsync(id);
            if (productDto == null)
            {
                return NotFound();
            }

            ViewModel = new ProductDetailViewModel
            {
                Id = productDto.Id,
                ShopId = productDto.ShopId,
                CategoryId = productDto.CategoryId,
                Name = productDto.Name,
                Description = productDto.Description,
                BasePrice = productDto.BasePrice,
                Status = productDto.Status,
                AvgRating = productDto.AvgRating,
                ImageUrl = productDto.ImageUrl,
                CreatedAt = productDto.CreatedAt,
                CategoryName = productDto.CategoryName,
                ShopName = productDto.ShopName,
                Variants = productDto
                    .Variants.Select(v => new ProductVariantItemViewModel
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
                Reviews =
                    (await _reviewService.GetReviewsByProductIdAsync(id))
                        ?.Select(r => new ReviewViewModel
                        {
                            Id = r.Id,
                            UserId = r.UserId,
                            UserName = r.UserName,
                            Rating = r.Rating,
                            Comment = r.Comment,
                            CreatedAt = r.CreatedAt,
                            ImageUrl = r.ImageUrl,
                            ModerateAt = r.ModeratedAt,
                        })
                        .ToList() ?? new List<ReviewViewModel>(),
            };

            return Page();
        }

        public async Task<IActionResult> OnPostCreatePreOrderAsync(
            Guid id,
            Guid productId,
            Guid productVariantId,
            int quantity,
            string shippingAddress,
            decimal? depositPercent
        )
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToPage(
                    "/Authentication/Login",
                    new { returnUrl = $"/Product/Detail?id={productId}" }
                );
            }

            try
            {
                var preOrder = await _preOrderService.CreateAsync(
                    userId,
                    new Service.DTOs.CreatePreOrderRequest
                    {
                        ProductId = productId,
                        VariantId = productVariantId,
                        Quantity = quantity,
                        ShippingAddress = shippingAddress,
                        DepositPercent = depositPercent,
                    }
                );

                TempData["PreOrderSuccessMessage"] = "Đã tạo đơn đặt trước thành công.";
                TempData["PreOrderCreatedOrderId"] = preOrder.OrderId.ToString();
                return RedirectToPage(
                    "/Product/Detail",
                    new { id = id == Guid.Empty ? productId : id }
                );
            }
            catch (Exception ex)
            {
                TempData["PreOrderErrorMessage"] = ex.Message;
                return RedirectToPage(
                    "/Product/Detail",
                    new { id = id == Guid.Empty ? productId : id }
                );
            }
        }
    }
}
