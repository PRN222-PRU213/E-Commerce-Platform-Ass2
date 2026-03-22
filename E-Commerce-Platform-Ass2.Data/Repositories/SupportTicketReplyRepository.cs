using E_Commerce_Platform_Ass2.Data.Database;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass2.Data.Repositories
{
    public class SupportTicketReplyRepository : ISupportTicketReplyRepository
    {
        private readonly ApplicationDbContext _context;

        public SupportTicketReplyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SupportTicketReply> CreateAsync(SupportTicketReply reply)
        {
            reply.Id = Guid.NewGuid();
            reply.CreatedAt = DateTime.UtcNow;
            await _context.SupportTicketReplies.AddAsync(reply);
            await _context.SaveChangesAsync();
            return reply;
        }

        public async Task<List<SupportTicketReply>> GetByTicketIdAsync(Guid ticketId)
        {
            return await _context.SupportTicketReplies
                .Include(x => x.Sender)
                .Where(x => x.TicketId == ticketId && !x.IsInternal)
                .OrderBy(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<SupportTicketReply?> GetByIdAsync(Guid id)
        {
            return await _context.SupportTicketReplies
                .Include(x => x.Sender)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<int> GetReplyCountByTicketIdAsync(Guid ticketId)
        {
            return await _context.SupportTicketReplies
                .CountAsync(x => x.TicketId == ticketId);
        }
    }
}
