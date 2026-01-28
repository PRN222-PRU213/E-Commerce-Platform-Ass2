using E_Commerce_Platform_Ass1.Data.Database;
using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Platform_Ass1.Data.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDbContext _context;

        public CartRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> CreateAsync(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
            return cart;
        }

        public async Task<bool> DeleteAsync(Cart cart)
        {
            _context.Carts.Remove(cart);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Cart?> GetByIdAsync(Guid cartId)
        {
            return await _context.Carts
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task<IEnumerable<Cart>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Carts
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public Task<Cart?> GetCartByUserIdAsync(Guid userId)
        {
            return _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.ProductVariant)
                .ThenInclude(pv => pv.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == "ACTIVE");
        }

        public async Task<decimal> GetCartTotalAsync(Guid userId)
        {
            return await _context.Carts
                .Where(c => c.UserId == userId && c.Status == "ACTIVE")
                .SelectMany(c => c.CartItems)
                .SumAsync(ci => ci.Quantity * ci.ProductVariant.Price);
        }

        public async Task<int> GetTotalItemCountAsync(Guid userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Status == "ACTIVE");
            if (cart == null)
            {
                return 0;
            }
            return cart.CartItems.Sum(ci => ci.Quantity);
        }

        public async Task<Cart> UpdateAsync(Cart cart)
        {
            _context.Carts.Update(cart);
            await _context.SaveChangesAsync();
            return cart;
        }
    }
}
