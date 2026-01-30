namespace E_Commerce_Platform_Ass2.Data.Database.Entities
{
    /// <summary>
    /// Lịch sử giao dịch ví Shop
    /// </summary>
    public class ShopWalletTransaction
    {
        public Guid Id { get; set; }
        public Guid ShopWalletId { get; set; }
        public Guid? OrderId { get; set; }
        
        /// <summary>
        /// Loại giao dịch: Sale (nhận từ đơn hàng), Refund (hoàn tiền cho khách), Withdrawal (rút tiền)
        /// </summary>
        public string TransactionType { get; set; } = string.Empty;
        
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public string? Description { get; set; }
        public string? ReferenceId { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation
        public ShopWallet ShopWallet { get; set; } = null!;
        public Order? Order { get; set; }
    }
}
