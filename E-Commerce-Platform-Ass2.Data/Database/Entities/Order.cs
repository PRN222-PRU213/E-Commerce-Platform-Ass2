namespace E_Commerce_Platform_Ass1.Data.Database.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public decimal TotalAmount { get; set; }

        public string ShippingAddress { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        // Navigation property
        public User User { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public ICollection<Shipment> Shipments { get; set; } = new List<Shipment>();

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
