namespace E_Commerce_Platform_Ass2.Data.Database.Entities
{
    public class PreOrderDetail
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }

        public Guid ShopId { get; set; }

        public DateTime ExpectedAvailableDate { get; set; }

        public decimal DepositPercent { get; set; }

        public decimal DepositAmount { get; set; }

        public decimal RemainingAmount { get; set; }

        public int FinalPaymentDeadlineHours { get; set; } = 48;

        public DateTime? ActivatedFinalPaymentAt { get; set; }

        public DateTime? ExpiredAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Order Order { get; set; } = null!;

        public Shop Shop { get; set; } = null!;
    }
}
