namespace E_Commerce_Platform_Ass1.Data.Database.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid ProductVariantId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int Quantity { get; set; }

        // Navigation property
        public Order Order { get; set; } = null!;

        public ProductVariant ProductVariant { get; set; } = null!;

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
