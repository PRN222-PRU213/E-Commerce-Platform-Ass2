using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IProductService _productService;

        public IndexModel(ILogger<IndexModel> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public HomeIndexViewModel ViewModel { get; set; } = new();

        public async Task OnGetAsync()
        {
            var products = await _productService.GetAllProductsAsync();
            ViewModel = new HomeIndexViewModel
            {
                Products = products
                    .Select(p => new HomeProductItemViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        BasePrice = p.BasePrice,
                        ImageUrl = p.ImageUrl,
                        AvgRating = p.AvgRating,
                        ShopName = p.ShopName,
                        CategoryName = p.CategoryName,
                    })
                    .ToList(),
            };
        }
    }
}
