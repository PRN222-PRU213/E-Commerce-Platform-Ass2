using E_Commerce_Platform_Ass2.Data.Database;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Platform_Ass2.Data.Repositories
{
    public class ShopWalletTransactionRepository : IShopWalletTransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public ShopWalletTransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ShopWalletTransaction transaction)
        {
            _context.ShopWalletTransactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ShopWalletTransaction>> GetByShopWalletIdAsync(Guid shopWalletId, int limit = 20)
        {
            return await _context.ShopWalletTransactions
                .Where(t => t.ShopWalletId == shopWalletId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(limit)
                .Include(t => t.Order)
                .ToListAsync();
        }
    }
}
