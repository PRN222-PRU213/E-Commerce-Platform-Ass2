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

        public WalletService(IWalletRepository walletRepository)
        {
            _walletRepository = walletRepository;
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
    }
}
