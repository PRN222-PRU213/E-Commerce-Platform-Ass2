namespace E_Commerce_Platform_Ass1.Data.Database.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public string Method { get; set; } = string.Empty;

        public decimal Amount { get; set; }

        public string Status { get; set; } = string.Empty;

        public string TransactionCode { get; set; } = string.Empty;

        public DateTime PaidAt { get; set; }

        // Navigation property
        public Order Order { get; set; } = null!;
    }
}
