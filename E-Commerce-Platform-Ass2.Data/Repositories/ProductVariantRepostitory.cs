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
    public class ProductVariantRepostitory : IProductVariantRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductVariantRepostitory(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProductVariant> AddAsync(ProductVariant productVariant)
        {
            _context.ProductVariants.Add(productVariant);
            await _context.SaveChangesAsync();
            return productVariant;
        }

        public async Task<bool> DeleteAsync(ProductVariant productVariant)
        {
            _context.ProductVariants.Remove(productVariant);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProductVariant>> GetAllAsync()
        {
            return await _context.ProductVariants.ToListAsync();
        }

        public async Task<ProductVariant?> GetByIdAsync(Guid id)
        {
            return await _context.ProductVariants.FindAsync(id);
        }

        public async Task<IEnumerable<ProductVariant>> GetByProductIdAsync(Guid productId)
        {
            return await _context.ProductVariants
                .Where(x => x.ProductId == productId)
                .ToListAsync();
        }

        public async Task<ProductVariant> UpdateAsync(ProductVariant productVariant)
        {
            _context.ProductVariants.Update(productVariant);
            await _context.SaveChangesAsync();
            return productVariant;
        }
    }
}
