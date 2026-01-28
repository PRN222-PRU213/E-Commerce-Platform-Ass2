namespace E_Commerce_Platform_Ass1.Data.Database.Entities
{
    public class CartItem
    {
        public Guid Id { get; set; }

        public Guid CartId { get; set; }

        public Guid ProductVariantId { get; set; }

        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation property
        public Cart Cart { get; set; } = null!;

        public ProductVariant ProductVariant { get; set; } = null!;

        public CartItem(Guid id, Guid cartId, Guid productVariantId, int quantity)
        {
            Id = id;
            CartId = cartId;
            ProductVariantId = productVariantId;
            Quantity = quantity;
            CreatedAt = DateTime.Now;
        }
    }
}
