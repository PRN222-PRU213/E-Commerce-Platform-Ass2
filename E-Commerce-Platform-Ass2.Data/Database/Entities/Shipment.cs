namespace E_Commerce_Platform_Ass1.Data.Database.Entities
{
    public class Shipment
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public string Carrier { get; set; } = string.Empty;

        public string TrackingCode { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime UpdatedAt { get; set; }

        // Navigation property
        public Order Order { get; set; } = null!;
    }
}
