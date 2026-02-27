namespace E_Commerce_Platform_Ass2.Data.Database.Entities
{
    public class Review
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid ProductId { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime? ModeratedAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public string ImageUrl { get; set; } = string.Empty;

        // Navigation Properties
        public User User { get; set; } = null!;

        public Product Product { get; set; } = null!;
    }
}
