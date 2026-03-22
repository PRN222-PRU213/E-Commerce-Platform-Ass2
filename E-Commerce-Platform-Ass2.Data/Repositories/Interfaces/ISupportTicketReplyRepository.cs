using E_Commerce_Platform_Ass2.Data.Database.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass2.Data.Repositories.Interfaces
{
    public interface ISupportTicketReplyRepository
    {
        Task<SupportTicketReply> CreateAsync(SupportTicketReply reply);
        Task<List<SupportTicketReply>> GetByTicketIdAsync(Guid ticketId);
        Task<SupportTicketReply?> GetByIdAsync(Guid id);
        Task<int> GetReplyCountByTicketIdAsync(Guid ticketId);
    }
}
