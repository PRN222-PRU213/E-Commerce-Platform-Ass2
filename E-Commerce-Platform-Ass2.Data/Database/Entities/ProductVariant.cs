namespace E_Commerce_Platform_Ass1.Data.Database.Entities
{
    public class ProductVariant
    {
        public Guid Id { get; set; }

        public Guid ProductId { get; set; }

        public string VariantName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public string Size { get; set; } = string.Empty;

        public string Color { get; set; } = string.Empty;

        public int Stock { get; set; }

        public string Sku { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public string ImageUrl { get; set; } = string.Empty;

        // Navigation property
        public Product Product { get; set; } = null!;

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
