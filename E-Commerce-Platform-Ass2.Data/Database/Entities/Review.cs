namespace E_Commerce_Platform_Ass1.Data.Database.Entities
{
    public class Review
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid ProductId { get; set; }

        public Guid OrderItemId { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public decimal SpamScore { get; set; }

        public decimal ToxicityScore { get; set; }

        public string ModerationReason { get; set; } = string.Empty;

        public DateTime? ModeratedAt { get; set; }

        public Guid? ModeratedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation Properties
        public User User { get; set; } = null!;

        public Product Product { get; set; } = null!;

        public OrderItem OrderItem { get; set; } = null!;
    }
}
