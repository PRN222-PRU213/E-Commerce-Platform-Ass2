using E_Commerce_Platform_Ass2.Data.Database;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Platform_Ass2.Data.Repositories
{
    public class PreOrderDetailRepository : IPreOrderDetailRepository
    {
        private static readonly string[] ActivePreOrderStatuses =
        {
            "DEPOSIT_PENDING",
            "DEPOSIT_PAID",
            "READY_FOR_FINAL_PAYMENT",
        };

        private readonly ApplicationDbContext _context;

        public PreOrderDetailRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PreOrderDetail> AddAsync(PreOrderDetail detail)
        {
            _context.PreOrderDetails.Add(detail);
            await _context.SaveChangesAsync();
            return detail;
        }

        public async Task<PreOrderDetail?> GetByIdAsync(Guid id)
        {
            return await _context.PreOrderDetails.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PreOrderDetail?> GetByIdWithOrderAsync(Guid id)
        {
            return await _context
                .PreOrderDetails.Include(x => x.Order)
                .ThenInclude(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                .ThenInclude(v => v.Product)
                .Include(x => x.Order)
                .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<PreOrderDetail>> GetByShopIdAsync(Guid shopId)
        {
            return await _context
                .PreOrderDetails.Include(x => x.Order)
                .ThenInclude(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                .ThenInclude(v => v.Product)
                .Where(x => x.ShopId == shopId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<PreOrderDetail>> GetByUserIdAsync(Guid userId)
        {
            return await _context
                .PreOrderDetails.Include(x => x.Order)
                .ThenInclude(o => o.OrderItems)
                .ThenInclude(oi => oi.ProductVariant)
                .ThenInclude(v => v.Product)
                .Where(x => x.Order.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetReservedQuantityByVariantAsync(Guid productVariantId)
        {
            return await _context
                    .PreOrderDetails.Where(x =>
                        x.Order.PreOrderStatus != null
                        && ActivePreOrderStatuses.Contains(x.Order.PreOrderStatus)
                    )
                    .SelectMany(x => x.Order.OrderItems)
                    .Where(oi => oi.ProductVariantId == productVariantId)
                    .SumAsync(oi => (int?)oi.Quantity) ?? 0;
        }

        public async Task<PreOrderDetail> UpdateAsync(PreOrderDetail detail)
        {
            _context.PreOrderDetails.Update(detail);
            await _context.SaveChangesAsync();
            return detail;
        }
    }
}
