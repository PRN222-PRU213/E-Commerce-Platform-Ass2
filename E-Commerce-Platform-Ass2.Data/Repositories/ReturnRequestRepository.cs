using E_Commerce_Platform_Ass2.Data.Database;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Platform_Ass2.Data.Repositories
{
    public class ReturnRequestRepository : IReturnRequestRepository
    {
        private readonly ApplicationDbContext _context;

        public ReturnRequestRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ReturnRequest?> GetByIdAsync(Guid id)
        {
            return await _context.ReturnRequests.FindAsync(id);
        }

        public async Task<ReturnRequest?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.ReturnRequests
                .Include(r => r.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.ProductVariant)
                            .ThenInclude(pv => pv.Product)
                .Include(r => r.User)
                .Include(r => r.ProcessedByShop)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<ReturnRequest?> GetByOrderIdAsync(Guid orderId)
        {
            return await _context.ReturnRequests
                .Include(r => r.Order)
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.OrderId == orderId);
        }

        public async Task<IEnumerable<ReturnRequest>> GetByUserIdAsync(Guid userId)
        {
            return await _context.ReturnRequests
                .Include(r => r.Order)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ReturnRequest>> GetByShopIdAsync(Guid shopId, string? status = null)
        {
            // Lấy các return request của orders thuộc shop này
            var query = _context.ReturnRequests
                .Include(r => r.Order)
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.ProductVariant)
                            .ThenInclude(pv => pv.Product)
                                .ThenInclude(p => p.Shop)
                .Include(r => r.User)
                .Where(r => r.Order.OrderItems.Any(oi => oi.ProductVariant.Product.ShopId == shopId));

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }

            return await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        }

        public async Task<bool> ExistsByOrderIdAsync(Guid orderId)
        {
            return await _context.ReturnRequests.AnyAsync(r => r.OrderId == orderId && r.Status != "Rejected");
        }

        public async Task AddAsync(ReturnRequest request)
        {
            await _context.ReturnRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ReturnRequest request)
        {
            _context.ReturnRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task<int> CountByOrderIdAsync(Guid orderId)
        {
            return await _context.ReturnRequests
                .CountAsync(r => r.OrderId == orderId);
        }

        public async Task<int> CountByUserIdThisMonthAsync(Guid userId)
        {
            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            return await _context.ReturnRequests
                .CountAsync(r => r.UserId == userId && r.CreatedAt >= startOfMonth);
        }
    }
}
