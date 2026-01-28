namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    /// <summary>
    /// DTO chi tiết sản phẩm (bao gồm variants) dùng cho trang Edit
    /// </summary>
    public class ProductDetailDto
    {
        public Guid Id { get; set; }
        public Guid ShopId { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal AvgRating { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? CategoryName { get; set; }
        public string? ShopName { get; set; }

        /// <summary>
        /// Danh sách biến thể của sản phẩm
        /// </summary>
        public List<ProductVariantDto> Variants { get; set; } = new();
    }
}
