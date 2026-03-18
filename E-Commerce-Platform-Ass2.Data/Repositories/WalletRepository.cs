using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Platform_Ass2.Data.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ApplicationDbContext _context;

        public WalletRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task<Wallet> GetByUserIdAsync(Guid userId)
        {
            return await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<Wallet> GetOrCreateAdminWalletAsync()
        {
            var adminWallet = await _context.Wallets
                .Include(w => w.User)
                .ThenInclude(u => u.Role)
                .FirstOrDefaultAsync(w => w.User.Role.Name == "Admin");

            if (adminWallet != null)
            {
                return adminWallet;
            }

            var adminUser = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Role.Name == "Admin");

            if (adminUser == null)
            {
                throw new InvalidOperationException("Không tìm thấy tài khoản Admin để tạo ví hệ thống.");
            }

            adminWallet = new Wallet
            {
                WalletId = Guid.NewGuid(),
                UserId = adminUser.Id,
                Balance = 0,
                LastChangeAmount = 0,
                LastChangeType = "Init",
                UpdatedAt = DateTime.UtcNow
            };

            _context.Wallets.Add(adminWallet);
            await _context.SaveChangesAsync();

            return adminWallet;
        }

        public async Task UpdateAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
        }
    }
}
