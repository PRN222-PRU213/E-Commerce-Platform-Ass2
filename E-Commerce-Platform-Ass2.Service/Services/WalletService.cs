using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletTransactionRepository _transactionRepository;

        public WalletService(IWalletRepository walletRepository, IWalletTransactionRepository transactionRepository)
        {
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<WalletDto> GetOrCreateAsync(Guid userId)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);

            if (wallet == null)
            {
                wallet = new Wallet
                {
                    WalletId = Guid.NewGuid(),
                    UserId = userId,
                    Balance = 0,
                    UpdatedAt = DateTime.UtcNow,
                    LastChangeType = "Init", // hoặc "Create"
                    LastChangeAmount = 0,
                };

                try
                {
                    await _walletRepository.AddAsync(wallet);
                }
                catch (DbUpdateException)
                {
                    // thử lấy lại lần nữa
                    wallet = await _walletRepository.GetByUserIdAsync(userId);

                    if (wallet == null)
                    {
                        throw new Exception("Không thể tạo hoặc lấy Wallet cho user này.");
                    }
                }
            }

            return new WalletDto
            {
                WalletId = wallet.WalletId,
                UserId = wallet.UserId,
                Balance = wallet.Balance,
                UpdatedAt = wallet.UpdatedAt,
                LastChangeAmount = wallet.LastChangeAmount,
                LastChangeType = wallet.LastChangeType
            };
        }


        public async Task<WalletPaymentResultDto> PayAsync(
            Guid userId,
            decimal orderTotal)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);

            if (wallet.Balance >= orderTotal)
            {
                wallet.Balance -= orderTotal;
                await _walletRepository.UpdateAsync(wallet);

                return new WalletPaymentResultDto
                {
                    WalletUsedAmount = orderTotal,
                    MomoPayAmount = 0,
                    NeedMomo = false
                };
            }

            var walletUsed = wallet.Balance;
            var momoAmount = orderTotal - walletUsed;

            wallet.Balance = 0;
            await _walletRepository.UpdateAsync(wallet);

            return new WalletPaymentResultDto
            {
                WalletUsedAmount = walletUsed,
                MomoPayAmount = momoAmount,
                NeedMomo = true
            };
        }


        public async Task RefundAsync(Guid userId, decimal amount)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);

            wallet.Balance += amount;
            wallet.LastChangeAmount = amount;
            wallet.LastChangeType = "REFUND";
            wallet.UpdatedAt = DateTime.Now;

            await _walletRepository.UpdateAsync(wallet);
        }

        /// <summary>
        /// Nạp tiền vào ví
        /// </summary>
        public async Task<WalletDto> TopUpAsync(Guid userId, decimal amount, string? referenceId = null)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            
            if (wallet == null)
            {
                wallet = new Wallet
                {
                    WalletId = Guid.NewGuid(),
                    UserId = userId,
                    Balance = 0,
                    UpdatedAt = DateTime.UtcNow
                };
                await _walletRepository.AddAsync(wallet);
            }
            
            wallet.Balance += amount;
            wallet.LastChangeAmount = amount;
            wallet.LastChangeType = "TopUp";
            wallet.UpdatedAt = DateTime.UtcNow;
            
            await _walletRepository.UpdateAsync(wallet);
            
            // Log transaction
            await _transactionRepository.AddAsync(new WalletTransaction
            {
                Id = Guid.NewGuid(),
                WalletId = wallet.WalletId,
                TransactionType = "TopUp",
                Amount = amount,
                BalanceAfter = wallet.Balance,
                Description = "Nạp tiền qua Momo",
                ReferenceId = referenceId,
                CreatedAt = DateTime.UtcNow
            });
            
            return new WalletDto
            {
                WalletId = wallet.WalletId,
                UserId = wallet.UserId,
                Balance = wallet.Balance,
                UpdatedAt = wallet.UpdatedAt,
                LastChangeAmount = wallet.LastChangeAmount,
                LastChangeType = wallet.LastChangeType
            };
        }
        
        /// <summary>
        /// Lấy lịch sử giao dịch ví
        /// </summary>
        public async Task<List<WalletTransactionDto>> GetTransactionsAsync(Guid userId, int take = 20)
        {
            var wallet = await _walletRepository.GetByUserIdAsync(userId);
            
            if (wallet == null)
            {
                return new List<WalletTransactionDto>();
            }
            
            var transactions = await _transactionRepository.GetByWalletIdAsync(wallet.WalletId, take);
            
            return transactions.Select(t => new WalletTransactionDto
            {
                Id = t.Id,
                TransactionType = t.TransactionType,
                Amount = t.Amount,
                BalanceAfter = t.BalanceAfter,
                Description = t.Description,
                ReferenceId = t.ReferenceId,
                CreatedAt = t.CreatedAt
            }).ToList();
        }
    }
}
