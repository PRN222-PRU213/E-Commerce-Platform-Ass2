namespace E_Commerce_Platform_Ass2.Wed.Models
{
    /// <summary>
    /// ViewModel cho Home Index page - danh sách sản phẩm
    /// </summary>
    public class HomeIndexViewModel
    {
        public List<HomeProductItemViewModel> Products { get; set; } = new();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 8;
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// ViewModel cho Product item trong Home page
    /// </summary>
    public class HomeProductItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public decimal AvgRating { get; set; }
        public string? ShopName { get; set; }
        public string? CategoryName { get; set; }
    }
}
