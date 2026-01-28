using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Data.Database;
using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Platform_Ass1.Data.Repositories
{
    public class ShopRepository : IShopRepository
    {
        private readonly ApplicationDbContext _context;

        public ShopRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Shop> AddAsync(Shop shop)
        {
            _context.Shops.Add(shop);
            await _context.SaveChangesAsync();
            return shop;
        }

        public async Task<bool> DeleteAsync(Shop shop)
        {
            _context.Shops.Remove(shop);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsByShopName(string shopName)
        {
            var shop = await _context.Shops.FirstOrDefaultAsync(s => s.ShopName == shopName);

            if (shop == null)
            {
                return false;
            }
            return true;
        }

        public async Task<bool> ExistsByUserId(Guid userId)
        {
            var shop = await _context.Shops.FirstOrDefaultAsync(s => s.UserId == userId);
            if (shop == null)
            {
                return false;
            }
            return true;
        }

        public async Task<IEnumerable<Shop>> GetAllAsync()
        {
            return await _context.Shops.ToListAsync();
        }

        public async Task<Shop?> GetByIdAsync(Guid id)
        {
            return await _context.Shops.FindAsync(id);
        }

        public async Task<Shop?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Shops.FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<IEnumerable<Shop>> GetByStatus(string status)
        {
            return await _context.Shops.Where(s => s.Status == status).ToListAsync();
        }

        public async Task<Shop> UpdateAsync(Shop shop)
        {
            _context.Shops.Update(shop);
            await _context.SaveChangesAsync();
            return shop;
        }
    }
}
