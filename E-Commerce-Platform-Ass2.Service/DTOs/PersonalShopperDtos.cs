namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    public class ShopperMessageDto
    {
        public string Role { get; set; } = "user"; // "user" or "assistant"
        public string Content { get; set; } = string.Empty;
    }

    public class PersonalShopperProductDto
    {
        public Guid ProductId { get; set; }
        public Guid VariantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public string? VariantName { get; set; }
        public string? Color { get; set; }
        public string? Size { get; set; }
    }

    public class ProductComboDto
    {
        public string Name { get; set; } = string.Empty;
        public string Style { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public List<PersonalShopperProductDto> Products { get; set; } = new();
    }

    public class ShopperChatRequest
    {
        public string Message { get; set; } = string.Empty;
        public List<ShopperMessageDto> History { get; set; } = new();
    }

    public class ShopperChatResponse
    {
        public string Message { get; set; } = string.Empty;
        public List<ProductComboDto>? Combos { get; set; }
    }

    public class AddComboToCartRequest
    {
        public List<Guid> VariantIds { get; set; } = new();
    }

    public class AddComboToCartResult
    {
        public int RequestedCount { get; set; }
        public int AddedCount { get; set; }
        public int SkippedInvalidCount { get; set; }
        public int SkippedInactiveCount { get; set; }
        public int SkippedOutOfStockCount { get; set; }
        public List<Guid> AddedVariantIds { get; set; } = new();
        public List<Guid> SkippedVariantIds { get; set; } = new();
        public string Message { get; set; } = string.Empty;
    }
}
