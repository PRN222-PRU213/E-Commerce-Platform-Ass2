using E_Commerce_Platform_Ass2.Service.DTOs;

namespace E_Commerce_Platform_Ass2.Wed.Models
{
    public class WalletViewModel
    {
        public decimal Balance { get; set; }
        public decimal? LastChangeAmount { get; set; }
        public string? LastChangeType { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<WalletTransactionDto> Transactions { get; set; } = new();
    }
}
