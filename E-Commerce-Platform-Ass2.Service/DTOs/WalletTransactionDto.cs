namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    /// <summary>
    /// DTO cho lịch sử giao dịch ví
    /// </summary>
    public class WalletTransactionDto
    {
        public Guid Id { get; set; }
        public string TransactionType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string? Description { get; set; }
        public string? ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
