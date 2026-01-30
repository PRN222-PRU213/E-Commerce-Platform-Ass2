using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Service.DTOs;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface IWalletService
    {
        Task<WalletDto> GetOrCreateAsync(Guid userId);
        Task RefundAsync(Guid userId, decimal amount);
        Task<WalletPaymentResultDto> PayAsync(Guid userId, decimal amount);
        
        /// <summary>
        /// Nạp tiền vào ví
        /// </summary>
        Task<WalletDto> TopUpAsync(Guid userId, decimal amount, string? referenceId = null);
        
        /// <summary>
        /// Lấy lịch sử giao dịch ví
        /// </summary>
        Task<List<WalletTransactionDto>> GetTransactionsAsync(Guid userId, int take = 20);
    }
}
