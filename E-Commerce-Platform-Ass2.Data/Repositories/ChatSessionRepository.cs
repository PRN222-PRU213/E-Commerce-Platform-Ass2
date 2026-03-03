using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using E_Commerce_Platform_Ass2.Data.Database;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;

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
            return await _context.ChatSessions
                .Include(cs => cs.Customer)
                .Include(cs => cs.Shop)
                .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(cs => cs.Id == id);
        }

        public async Task<ChatSession?> GetByCustomerAndShopAsync(Guid customerId, Guid shopId)
        {
            return await _context.ChatSessions
                .FirstOrDefaultAsync(cs => cs.CustomerId == customerId && cs.ShopId == shopId);
        }

        public async Task<IEnumerable<ChatSession>> GetSessionsByCustomerAsync(Guid customerId)
        {
            return await _context.ChatSessions
                .Include(cs => cs.Shop)
                .Where(cs => cs.CustomerId == customerId)
                .OrderByDescending(cs => cs.UpdatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ChatSession>> GetSessionsByShopAsync(Guid shopId)
        {
            return await _context.ChatSessions
                .Include(cs => cs.Customer)
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

        public async Task UpdateAsync(ChatSession session)
        {
            _context.ChatSessions.Update(session);
            await _context.SaveChangesAsync();
        }
    }
}
