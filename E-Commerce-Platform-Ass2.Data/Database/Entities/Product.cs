namespace E_Commerce_Platform_Ass1.Data.Database.Entities
{
    public class Product
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

        // Navigation property
        public Category Category { get; set; } = null!;

        public Shop Shop { get; set; } = null!;

        public ICollection<ProductVariant> ProductVariants { get; set; } = new List<ProductVariant>();

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
