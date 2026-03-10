using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_Platform_Ass2.Data.Repositories
{
    public class ChatSessionRepository : IChatSessionRepository
    {
        private readonly ApplicationDbContext _context;

        public ChatSessionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ChatSession?> GetByIdAsync(Guid id)
        {
            return await _context
                .ChatSessions.Include(cs => cs.Customer)
                .Include(cs => cs.Shop)
                .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(cs => cs.Id == id);
        }

        public async Task<ChatSession?> GetByCustomerAndShopAsync(Guid customerId, Guid shopId)
        {
            return await _context.ChatSessions.FirstOrDefaultAsync(cs =>
                cs.CustomerId == customerId && cs.ShopId == shopId
            );
        }

        public async Task<IEnumerable<ChatSession>> GetSessionsByCustomerAsync(Guid customerId)
        {
            return await _context
                .ChatSessions.Include(cs => cs.Shop)
                .Where(cs => cs.CustomerId == customerId)
                .OrderByDescending(cs => cs.UpdatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChatSession>> GetSessionsByShopAsync(Guid shopId)
        {
            return await _context
                .ChatSessions.Include(cs => cs.Customer)
                .Include(cs => cs.Messages)
                .Where(cs => cs.ShopId == shopId)
                .OrderByDescending(cs => cs.UpdatedAt)
                .ToListAsync();
        }

        public async Task<ChatSession> CreateAsync(ChatSession session)
        {
            _context.ChatSessions.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task UpdateTimestampAsync(Guid sessionId)
        {
            await _context
                .ChatSessions.Where(s => s.Id == sessionId)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.UpdatedAt, DateTime.UtcNow));
        }

        public async Task UpdateAsync(ChatSession session)
        {
            // Use Entry directly to mark only the root entity as Modified.
            // DbContext.Update() traverses the full navigation graph and also marks
            // related User/Shop entities as Modified, which causes unnecessary UPDATEs
            // and can trigger database constraint violations.
            _context.Entry(session).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
