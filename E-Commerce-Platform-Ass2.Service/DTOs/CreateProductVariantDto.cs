namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    /// <summary>
    /// DTO để tạo biến thể sản phẩm mới
    /// </summary>
    public class CreateProductVariantDto
    {
        public Guid ProductId { get; set; }
        public string VariantName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public int Stock { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}
