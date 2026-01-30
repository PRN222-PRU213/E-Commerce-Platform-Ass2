using E_Commerce_Platform_Ass2.Data.Database;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Platform_Ass2.Data.Repositories
{
    public class ShopWalletRepository : IShopWalletRepository
    {
        private readonly ApplicationDbContext _context;

        public ShopWalletRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ShopWallet?> GetByShopIdAsync(Guid shopId)
        {
            return await _context.ShopWallets
                .FirstOrDefaultAsync(w => w.ShopId == shopId);
        }

        public async Task<ShopWallet> GetOrCreateAsync(Guid shopId)
        {
            var wallet = await GetByShopIdAsync(shopId);
            if (wallet != null)
                return wallet;

            wallet = new ShopWallet
            {
                Id = Guid.NewGuid(),
                ShopId = shopId,
                Balance = 0,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.ShopWallets.Add(wallet);
            await _context.SaveChangesAsync();

            return wallet;
        }

        public async Task UpdateAsync(ShopWallet wallet)
        {
            wallet.UpdatedAt = DateTime.Now;
            _context.ShopWallets.Update(wallet);
            await _context.SaveChangesAsync();
        }
    }
}
