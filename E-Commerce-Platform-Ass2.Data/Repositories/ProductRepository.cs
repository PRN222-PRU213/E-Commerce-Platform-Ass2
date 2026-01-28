using E_Commerce_Platform_Ass1.Data.Database;
using E_Commerce_Platform_Ass1.Data.Database.Entities;
using E_Commerce_Platform_Ass1.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Platform_Ass1.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteAsync(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(Guid productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return false;
            }
            return true;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context
                .Products.Include(p => p.Category)
                .Include(p => p.Shop)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId)
        {
            return await _context.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(Guid id)
        {
            return await _context.Products.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetByShopIdAsync(Guid shopId)
        {
            return await _context
                .Products.Include(p => p.Category)
                .Where(p => p.ShopId == shopId)
                .ToListAsync();
        }

        public async Task<Product?> GetProductWithVariantsAsync(Guid id)
        {
            return await _context
                .Products.Include(p => p.ProductVariants)
                .Include(p => p.Category)
                .Include(p => p.Shop)
                .Include(p => p.Reviews)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }
    }
}
