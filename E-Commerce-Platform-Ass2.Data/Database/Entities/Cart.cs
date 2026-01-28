namespace E_Commerce_Platform_Ass1.Data.Database.Entities
{
    public class Cart
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        // Navigation property
        public User User { get; set; } = null!;

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        public Cart(Guid id, Guid userId)
        {
            Id = id;
            UserId = userId;
            Status = "ACTIVE";
            CreatedAt = DateTime.Now;
        }
    }
}
