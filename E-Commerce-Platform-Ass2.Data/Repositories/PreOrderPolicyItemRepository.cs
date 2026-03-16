using E_Commerce_Platform_Ass2.Data.Database;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Platform_Ass2.Data.Repositories
{
    public class PreOrderPolicyItemRepository : IPreOrderPolicyItemRepository
    {
        private readonly ApplicationDbContext _context;

        public PreOrderPolicyItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PreOrderPolicyItem> AddAsync(PreOrderPolicyItem item)
        {
            _context.PreOrderPolicyItems.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<PreOrderPolicyItem?> GetByVariantIdAsync(Guid productVariantId)
        {
            return await _context.PreOrderPolicyItems.FirstOrDefaultAsync(x =>
                x.ProductVariantId == productVariantId && x.Status == "Active"
            );
        }

        public async Task<List<PreOrderPolicyItem>> GetActiveByProductIdAsync(Guid productId)
        {
            return await _context
                .PreOrderPolicyItems.Include(x => x.ProductVariant)
                .Where(x =>
                    x.Status == "Active"
                    && x.AllowPreOrder
                    && x.ProductVariant.ProductId == productId
                )
                .ToListAsync();
        }

        public async Task<PreOrderPolicyItem> UpdateAsync(PreOrderPolicyItem item)
        {
            _context.PreOrderPolicyItems.Update(item);
            await _context.SaveChangesAsync();
            return item;
        }
    }
}
