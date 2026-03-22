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
    public class TicketAssignmentRuleRepository : ITicketAssignmentRuleRepository
    {
        private readonly ApplicationDbContext _context;

        public TicketAssignmentRuleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<TicketAssignmentRule> CreateAsync(TicketAssignmentRule rule)
        {
            rule.Id = Guid.NewGuid();
            rule.CreatedAt = DateTime.UtcNow;
            await _context.TicketAssignmentRules.AddAsync(rule);
            await _context.SaveChangesAsync();
            return rule;
        }

        public async Task<TicketAssignmentRule?> GetByIdAsync(Guid id)
        {
            return await _context.TicketAssignmentRules
                .Include(x => x.CreatedBy)
                .Include(x => x.AssignedTo)
                .Include(x => x.AssignedToRole)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<TicketAssignmentRule>> GetAllActiveAsync()
        {
            return await _context.TicketAssignmentRules
                .Include(x => x.CreatedBy)
                .Include(x => x.AssignedTo)
                .Include(x => x.AssignedToRole)
                .Where(x => x.IsActive)
                .OrderBy(x => x.Priority_Order)
                .ToListAsync();
        }

        public async Task<TicketAssignmentRule?> GetMatchingRuleAsync(string category, string priority)
        {
            return await _context.TicketAssignmentRules
                .Include(x => x.AssignedTo)
                .Include(x => x.AssignedToRole)
                .Where(x => x.IsActive)
                .Where(x => x.Category == category || x.Category == "All")
                .Where(x => x.Priority == priority || x.Priority == "All")
                .OrderBy(x => x.Priority_Order)
                .FirstOrDefaultAsync();
        }

        public async Task<TicketAssignmentRule> UpdateAsync(TicketAssignmentRule rule)
        {
            rule.UpdatedAt = DateTime.UtcNow;
            _context.TicketAssignmentRules.Update(rule);
            await _context.SaveChangesAsync();
            return rule;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var rule = await _context.TicketAssignmentRules.FindAsync(id);
            if (rule == null) return false;
            _context.TicketAssignmentRules.Remove(rule);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
