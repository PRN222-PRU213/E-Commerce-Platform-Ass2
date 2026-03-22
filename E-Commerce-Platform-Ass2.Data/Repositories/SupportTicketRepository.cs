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
    public class SupportTicketRepository : ISupportTicketRepository
    {
        private readonly ApplicationDbContext _context;

        public SupportTicketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<SupportTicket> CreateAsync(SupportTicket ticket)
        {
            ticket.Id = Guid.NewGuid();
            ticket.CreatedAt = DateTime.UtcNow;
            ticket.Status = "Open";
            ticket.ResponseCount = 0;
            await _context.SupportTickets.AddAsync(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<SupportTicket?> GetByIdAsync(Guid id)
        {
            return await _context.SupportTickets.FindAsync(id);
        }

        public async Task<SupportTicket?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _context.SupportTickets
                .Include(x => x.Customer)
                .Include(x => x.AssignedTo)
                .Include(x => x.RelatedShop)
                .Include(x => x.RelatedOrder)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<SupportTicket?> GetByTicketCodeAsync(string ticketCode)
        {
            return await _context.SupportTickets
                .Include(x => x.Customer)
                .Include(x => x.AssignedTo)
                .Include(x => x.RelatedShop)
                .FirstOrDefaultAsync(x => x.TicketCode == ticketCode);
        }

        public async Task<List<SupportTicket>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _context.SupportTickets
                .Include(x => x.Customer)
                .Include(x => x.RelatedShop)
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<SupportTicket>> GetByAssignedToIdAsync(Guid assignedToId)
        {
            return await _context.SupportTickets
                .Include(x => x.Customer)
                .Include(x => x.RelatedShop)
                .Where(x => x.AssignedToId == assignedToId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<SupportTicket>> GetByShopIdAsync(Guid shopId)
        {
            return await _context.SupportTickets
                .Include(x => x.Customer)
                .Include(x => x.RelatedShop)
                .Where(x => x.RelatedShopId == shopId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<SupportTicket>> GetAllAsync(string? status, string? priority, string? category)
        {
            var query = _context.SupportTickets
                .Include(x => x.Customer)
                .Include(x => x.AssignedTo)
                .Include(x => x.RelatedShop)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(x => x.Status == status);
            }

            if (!string.IsNullOrEmpty(priority) && priority != "All")
            {
                query = query.Where(x => x.Priority == priority);
            }

            if (!string.IsNullOrEmpty(category) && category != "All")
            {
                query = query.Where(x => x.Category == category);
            }

            return await query
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<SupportTicket> UpdateAsync(SupportTicket ticket)
        {
            ticket.UpdatedAt = DateTime.UtcNow;
            _context.SupportTickets.Update(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var ticket = await _context.SupportTickets.FindAsync(id);
            if (ticket == null) return false;
            _context.SupportTickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<string> GenerateTicketCodeAsync()
        {
            var today = DateTime.UtcNow.ToString("yyyyMMdd");
            var prefix = $"TK-{today}-";

            var lastTicket = await _context.SupportTickets
                .Where(x => x.TicketCode.StartsWith(prefix))
                .OrderByDescending(x => x.TicketCode)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (lastTicket != null)
            {
                var lastCode = lastTicket.TicketCode;
                var parts = lastCode.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastNum))
                {
                    nextNumber = lastNum + 1;
                }
            }

            return $"{prefix}{nextNumber:D4}";
        }

        public async Task<int> GetCountByStatusAsync(string status)
        {
            return await _context.SupportTickets.CountAsync(x => x.Status == status);
        }

        public async Task<int> GetTodayCountAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.SupportTickets.CountAsync(x => x.CreatedAt >= today);
        }

        public async Task<int> GetThisWeekCountAsync()
        {
            var startOfWeek = DateTime.UtcNow.Date.AddDays(-(int)DateTime.UtcNow.DayOfWeek);
            return await _context.SupportTickets.CountAsync(x => x.CreatedAt >= startOfWeek);
        }

        public async Task<double> GetAverageResolutionTimeAsync()
        {
            var resolvedTickets = await _context.SupportTickets
                .Where(x => x.ResolvedAt != null && x.ClosedAt != null)
                .Select(x => new
                {
                    ResolutionTime = (x.ClosedAt!.Value - x.CreatedAt).TotalHours
                })
                .ToListAsync();

            if (!resolvedTickets.Any()) return 0;
            return resolvedTickets.Average(x => x.ResolutionTime);
        }

        public async Task<Dictionary<string, int>> GetCountByCategoryAsync()
        {
            return await _context.SupportTickets
                .GroupBy(x => x.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Category, x => x.Count);
        }

        public async Task<Dictionary<string, int>> GetCountByPriorityAsync()
        {
            return await _context.SupportTickets
                .GroupBy(x => x.Priority)
                .Select(g => new { Priority = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Priority, x => x.Count);
        }

        public async Task<List<SupportTicket>> GetSlaAtRiskAsync()
        {
            var now = DateTime.UtcNow;
            var riskThreshold = TimeSpan.FromMinutes(30);

            return await _context.SupportTickets
                .Include(x => x.Customer)
                .Include(x => x.AssignedTo)
                .Where(x => x.Status != "Closed" && x.Status != "Resolved")
                .Where(x => x.SlaStatus == "OnTrack")
                .Where(x => x.FirstResponseDeadline != null &&
                           x.FirstResponseDeadline.Value - now < riskThreshold)
                .ToListAsync();
        }

        public async Task<List<SupportTicket>> GetSlaBreachedAsync()
        {
            var now = DateTime.UtcNow;

            return await _context.SupportTickets
                .Include(x => x.Customer)
                .Include(x => x.AssignedTo)
                .Where(x => x.Status != "Closed" && x.Status != "Resolved")
                .Where(x => x.FirstResponseDeadline != null &&
                           x.FirstResponseDeadline.Value < now &&
                           !x.FirstResponseMet)
                .ToListAsync();
        }

        public async Task<List<SupportTicket>> GetUnassignedAsync()
        {
            return await _context.SupportTickets
                .Include(x => x.Customer)
                .Where(x => x.AssignedToId == null && x.Status == "Open")
                .OrderByDescending(x => x.Priority == "Urgent")
                .ThenByDescending(x => x.Priority == "High")
                .ThenByDescending(x => x.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<SupportTicket>> GetEscalationEligibleAsync(int level)
        {
            var now = DateTime.UtcNow;

            return await _context.SupportTickets
                .Include(x => x.Customer)
                .Include(x => x.AssignedTo)
                .Where(x => x.Status != "Closed" && x.Status != "Resolved")
                .Where(x => x.EscalationLevel == level)
                .Where(x => x.FirstResponseDeadline != null &&
                           x.FirstResponseDeadline.Value < now &&
                           !x.FirstResponseMet)
                .ToListAsync();
        }

        public async Task<int> GetCountBySlaStatusAsync(string slaStatus)
        {
            return await _context.SupportTickets
                .CountAsync(x => x.SlaStatus == slaStatus &&
                                x.Status != "Closed" && x.Status != "Resolved");
        }
    }
}
