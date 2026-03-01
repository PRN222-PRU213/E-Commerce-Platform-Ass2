using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IProductService _productService;
        private const int PageSize = 8;

        public IndexModel(ILogger<IndexModel> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        public HomeIndexViewModel ViewModel { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string? SearchKeyword { get; set; }

        public async Task OnGetAsync()
        {
            var allProducts = await _productService.GetAllProductsAsync();

            // Lọc theo từ khóa tìm kiếm (case-insensitive)
            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                var keyword = SearchKeyword.Trim().ToLower();
                allProducts = allProducts
                    .Where(p =>
                        p.Name.ToLower().Contains(keyword) ||
                        (p.Description != null && p.Description.ToLower().Contains(keyword)) ||
                        (p.CategoryName != null && p.CategoryName.ToLower().Contains(keyword)) ||
                        (p.ShopName != null && p.ShopName.ToLower().Contains(keyword)))
                    .ToList();
            }

            var totalCount = allProducts.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);

            if (CurrentPage < 1) CurrentPage = 1;
            if (CurrentPage > totalPages && totalPages > 0) CurrentPage = totalPages;

            var pagedProducts = allProducts
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            ViewModel = new HomeIndexViewModel
            {
                Products = pagedProducts
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
                CurrentPage = CurrentPage,
                TotalPages = totalPages,
                PageSize = PageSize,
                TotalCount = totalCount,
                SearchKeyword = SearchKeyword,
            };
        }

        /// <summary>
        /// AJAX handler: trả về partial HTML cho lưới sản phẩm (không reload toàn trang).
        /// Gọi bằng: GET /?handler=ProductsPartial&SearchKeyword=...&currentpage=...
        /// </summary>
        public async Task<IActionResult> OnGetProductsPartialAsync()
        {
            var allProducts = await _productService.GetAllProductsAsync();

            if (!string.IsNullOrWhiteSpace(SearchKeyword))
            {
                var keyword = SearchKeyword.Trim().ToLower();
                allProducts = allProducts
                    .Where(p =>
                        p.Name.ToLower().Contains(keyword) ||
                        (p.Description != null && p.Description.ToLower().Contains(keyword)) ||
                        (p.CategoryName != null && p.CategoryName.ToLower().Contains(keyword)) ||
                        (p.ShopName != null && p.ShopName.ToLower().Contains(keyword)))
                    .ToList();
            }

            var totalCount = allProducts.Count;
            var totalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            if (CurrentPage < 1) CurrentPage = 1;
            if (CurrentPage > totalPages && totalPages > 0) CurrentPage = totalPages;

            var pagedProducts = allProducts
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToList();

            var vm = new HomeIndexViewModel
            {
                Products = pagedProducts
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
                CurrentPage = CurrentPage,
                TotalPages = totalPages,
                PageSize = PageSize,
                TotalCount = totalCount,
                SearchKeyword = SearchKeyword,
            };

            return Partial("Partials/_ProductGrid", vm);
        }
    }
}
