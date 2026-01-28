namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    /// <summary>
    /// DTO hiển thị thông tin Shop
    /// </summary>
    public class ShopDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerEmail { get; set; }
        public int ProductCount { get; set; }
    }

    /// <summary>
    /// DTO chi tiết Shop (bao gồm thêm thông tin)
    /// </summary>
    public class ShopDetailDto : ShopDto
    {
        public List<ProductDto> Products { get; set; } = new();
    }
}
