namespace E_Commerce_Platform_Ass1.Data.Database.Entities
{
    public class Shop
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string ShopName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        // Navigation property
        public ICollection<Product> Products { get; set; } = new List<Product>();

        public User User { get; set; } = null!;
    }
}
