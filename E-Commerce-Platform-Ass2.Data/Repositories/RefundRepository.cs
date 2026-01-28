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
    public class RefundRepository : IRefundRepository
    {
        private readonly ApplicationDbContext _context;

        public RefundRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Refund refund)
        {
            _context.Refunds.Add(refund);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsRequestIdAsync(string requestId)
        {
            return await _context.Refunds.AnyAsync(r => r.RequestId == requestId);
        }
    }
}
