using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Product
{
    public class DetailModel : PageModel
    {
        private readonly IProductService _productService;

        public DetailModel(IProductService productService)
        {
            _productService = productService;
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
            };

            return Page();
        }
    }
}
