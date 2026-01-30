namespace E_Commerce_Platform_Ass2.Data.Database.Entities
{
    /// <summary>
    /// Ví của Shop - nhận tiền từ đơn hàng
    /// </summary>
    public class ShopWallet
    {
        public Guid Id { get; set; }
        public Guid ShopId { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public Shop Shop { get; set; } = null!;
        public ICollection<ShopWalletTransaction> Transactions { get; set; } = new List<ShopWalletTransaction>();
    }
}
