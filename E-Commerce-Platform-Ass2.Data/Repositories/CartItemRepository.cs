using E_Commerce_Platform_Ass1.Data.Database;
using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Platform_Ass1.Data.Repositories
{
    public class CartItemRepository : ICartItemRepository
    {
        private readonly ApplicationDbContext _context;

        public CartItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByIdsAsync(IEnumerable<Guid> cartItemIds)
        {
            var items = await _context.CartItems
                .Where(ci => cartItemIds.Contains(ci.Id))
                .ToListAsync();

            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task<CartItem?> GetByCartAndVariantAsync(Guid cartId, Guid productVariantId)
        {
            return await _context.CartItems
                .FirstOrDefaultAsync(c => c.CartId == cartId
                                       && c.ProductVariantId == productVariantId);
        }

        public async Task<IEnumerable<CartItem>> GetByCartIdAsync(Guid cartId)
        {
            return await _context.CartItems
                .Where(c => c.CartId == cartId)
                .ToListAsync();
        }

        public async Task<CartItem?> GetByIdAsync(Guid id)
        {
            return await _context.CartItems
                .Include(c => c.Cart)
                .Include(ci => ci.ProductVariant)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CartItem>> GetItemByIdsAsync(IEnumerable<Guid> cartItemIds)
        {
            return await _context.CartItems
                .Where(ci => cartItemIds.Contains(ci.Id))
                .Include(ci => ci.ProductVariant)
                .ToListAsync();
        }

        public async Task UpdateAsync(CartItem cartItem)
        {
            _context.CartItems.Update(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(Guid cartItemId, int quantity)
        {
            var item = await _context.CartItems.FindAsync(cartItemId);
            if (item != null)
            {
                item.Quantity = quantity;
            }
            await _context.SaveChangesAsync();
        }
    }
}
