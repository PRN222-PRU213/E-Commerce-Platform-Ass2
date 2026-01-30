namespace E_Commerce_Platform_Ass2.Data.Database.Entities
{
    /// <summary>
    /// Lịch sử giao dịch ví
    /// </summary>
    public class WalletTransaction
    {
        public Guid Id { get; set; }
        public Guid WalletId { get; set; }
        
        /// <summary>
        /// Loại giao dịch: TopUp, Payment, Refund
        /// </summary>
        public string TransactionType { get; set; } = string.Empty;
        
        /// <summary>
        /// Số tiền giao dịch (dương = cộng, âm = trừ)
        /// </summary>
        public decimal Amount { get; set; }
        
        /// <summary>
        /// Số dư sau giao dịch
        /// </summary>
        public decimal BalanceAfter { get; set; }
        
        /// <summary>
        /// Mô tả giao dịch
        /// </summary>
        public string? Description { get; set; }
        
        /// <summary>
        /// Mã tham chiếu (orderId, momoTransId, etc.)
        /// </summary>
        public string? ReferenceId { get; set; }
        
        public DateTime CreatedAt { get; set; }

        // Navigation
        public Wallet Wallet { get; set; } = null!;
    }
}
