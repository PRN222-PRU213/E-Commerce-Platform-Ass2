using E_Commerce_Platform_Ass2.Data.Database.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass2.Data.Repositories.Interfaces
{
    public interface ISupportTicketRepository
    {
        Task<SupportTicket> CreateAsync(SupportTicket ticket);
        Task<SupportTicket?> GetByIdAsync(Guid id);
        Task<SupportTicket?> GetByIdWithDetailsAsync(Guid id);
        Task<SupportTicket?> GetByTicketCodeAsync(string ticketCode);
        Task<List<SupportTicket>> GetByCustomerIdAsync(Guid customerId);
        Task<List<SupportTicket>> GetByAssignedToIdAsync(Guid assignedToId);
        Task<List<SupportTicket>> GetByShopIdAsync(Guid shopId);
        Task<List<SupportTicket>> GetAllAsync(string? status, string? priority, string? category);
        Task<SupportTicket> UpdateAsync(SupportTicket ticket);
        Task<bool> DeleteAsync(Guid id);
        Task<string> GenerateTicketCodeAsync();
        Task<int> GetCountByStatusAsync(string status);
        Task<int> GetTodayCountAsync();
        Task<int> GetThisWeekCountAsync();
        Task<double> GetAverageResolutionTimeAsync();
        Task<Dictionary<string, int>> GetCountByCategoryAsync();
        Task<Dictionary<string, int>> GetCountByPriorityAsync();

        // SLA Methods
        Task<List<SupportTicket>> GetSlaAtRiskAsync();
        Task<List<SupportTicket>> GetSlaBreachedAsync();
        Task<List<SupportTicket>> GetUnassignedAsync();
        Task<List<SupportTicket>> GetEscalationEligibleAsync(int level);

        // Statistics
        Task<int> GetCountBySlaStatusAsync(string slaStatus);
    }
}
