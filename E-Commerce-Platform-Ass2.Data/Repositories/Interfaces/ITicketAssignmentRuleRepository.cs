using E_Commerce_Platform_Ass2.Data.Database.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass2.Data.Repositories.Interfaces
{
    public interface ITicketAssignmentRuleRepository
    {
        Task<TicketAssignmentRule> CreateAsync(TicketAssignmentRule rule);
        Task<TicketAssignmentRule?> GetByIdAsync(Guid id);
        Task<List<TicketAssignmentRule>> GetAllActiveAsync();
        Task<TicketAssignmentRule?> GetMatchingRuleAsync(string category, string priority);
        Task<TicketAssignmentRule> UpdateAsync(TicketAssignmentRule rule);
        Task<bool> DeleteAsync(Guid id);
    }
}
