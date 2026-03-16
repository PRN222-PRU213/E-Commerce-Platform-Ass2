namespace E_Commerce_Platform_Ass2.Data.Database.Entities
{
    public class PreOrderPolicyItem
    {
        public Guid Id { get; set; }

        public Guid ProductVariantId { get; set; }

        public bool AllowPreOrder { get; set; }

        public decimal? DepositPercent { get; set; }

        public int? MaxPreOrderQty { get; set; }

        public int? LeadTimeDays { get; set; }

        public string Status { get; set; } = "Active";

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ProductVariant ProductVariant { get; set; } = null!;
    }
}
