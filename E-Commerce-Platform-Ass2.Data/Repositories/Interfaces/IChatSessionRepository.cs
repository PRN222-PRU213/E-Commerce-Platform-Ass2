using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database.Entities;

namespace E_Commerce_Platform_Ass2.Data.Repositories.Interfaces
{
    public interface IChatSessionRepository
    {
        Task<ChatSession?> GetByIdAsync(Guid id);
        Task<ChatSession?> GetByCustomerAndShopAsync(Guid customerId, Guid shopId);
        Task<IEnumerable<ChatSession>> GetSessionsByCustomerAsync(Guid customerId);
        Task<IEnumerable<ChatSession>> GetSessionsByShopAsync(Guid shopId);
        Task<ChatSession> CreateAsync(ChatSession session);
        Task UpdateAsync(ChatSession session);

        /// <summary>Updates only the UpdatedAt timestamp without loading navigation properties.</summary>
        Task UpdateTimestampAsync(Guid sessionId);
    }
}
