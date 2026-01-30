using E_Commerce_Platform_Ass2.Service.DTOs;

namespace E_Commerce_Platform_Ass2.Wed.Models
{
    public class ShopWalletViewModel
    {
        public Guid ShopId { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public List<ShopWalletTransactionDto> RecentTransactions { get; set; } = new();
    }
}
