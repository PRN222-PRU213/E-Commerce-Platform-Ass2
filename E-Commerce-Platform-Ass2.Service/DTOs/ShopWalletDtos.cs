namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    public class ShopWalletDto
    {
        public Guid Id { get; set; }
        public Guid ShopId { get; set; }
        public decimal Balance { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ShopWalletTransactionDto
    {
        public Guid Id { get; set; }
        public Guid? OrderId { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string? Description { get; set; }
        public string? ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
