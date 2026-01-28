namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    public class CartItemViewModel
    {
        public Guid CartItemId { get; set; }

        public Guid ProductVariantId { get; set; }

        public Guid ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string VariantName { get; set; } = string.Empty;

        public string Size { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        public int Stock { get; set; }

        public decimal Price { get; set; }

        public decimal TotalLinePrice => Price * Stock;
    }
}
