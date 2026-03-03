using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database.Entities;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface IChatService
    {
        Task<ChatSession> GetOrCreateSessionAsync(Guid customerId, Guid shopId);
        Task<IEnumerable<ChatSession>> GetSessionsForCustomerAsync(Guid customerId);
        Task<IEnumerable<ChatSession>> GetSessionsForShopAsync(Guid shopId);
        Task<IEnumerable<ChatMessage>> GetMessagesAsync(Guid sessionId);
        Task<ChatMessage> SendMessageAsync(Guid sessionId, Guid? senderId, string senderRole, string content, Guid? productId = null);
        Task<ChatSession?> GetSessionByIdAsync(Guid sessionId);
    }
}
