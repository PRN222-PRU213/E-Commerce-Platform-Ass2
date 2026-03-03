using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database.Entities;

namespace E_Commerce_Platform_Ass2.Data.Repositories.Interfaces
{
    public interface IChatMessageRepository
    {
        Task<IEnumerable<ChatMessage>> GetMessagesBySessionIdAsync(Guid sessionId);
        Task<ChatMessage> CreateAsync(ChatMessage message);
        Task UpdateAsync(ChatMessage message);
    }
}
