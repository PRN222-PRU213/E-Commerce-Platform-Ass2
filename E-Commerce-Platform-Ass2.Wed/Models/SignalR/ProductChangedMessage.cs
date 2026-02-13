namespace E_Commerce_Platform_Ass2.Wed.Models.SignalR
{
    public class ProductChangedMessage
    {
        public Guid ProductId { get; set; }
        public Guid ShopId { get; set; }
        public string ChangeType { get; set; } = string.Empty; // created | updated | statusChanged | deleted
        public string? Status { get; set; }
        public string? Name { get; set; }
        public string? TriggeredBy { get; set; }
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

        // Additional fields for admin real-time display
        public string? ImageUrl { get; set; }
        public decimal BasePrice { get; set; }
        public string? CategoryName { get; set; }
        public string? ShopName { get; set; }
        public string? Description { get; set; }
    }
}
