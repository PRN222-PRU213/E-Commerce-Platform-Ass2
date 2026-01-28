namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    /// <summary>
    /// DTO để cập nhật thông tin sản phẩm
    /// </summary>
    public class UpdateProductDto
    {
        public Guid ProductId { get; set; }
        public Guid ShopId { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
