using E_Commerce_Platform_Ass2.Data.Database;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Platform_Ass2.Data.Repositories
{
    public class WalletTransactionRepository : IWalletTransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public WalletTransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(WalletTransaction transaction)
        {
            _context.WalletTransactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<List<WalletTransaction>> GetByWalletIdAsync(Guid walletId, int take = 20)
        {
            return await _context.WalletTransactions
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(take)
                .ToListAsync();
        }
    }
}
