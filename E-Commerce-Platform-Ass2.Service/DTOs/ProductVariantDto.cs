namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    /// <summary>
    /// DTO hiển thị thông tin biến thể sản phẩm
    /// </summary>
    public class ProductVariantDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string VariantName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public int Stock { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}
