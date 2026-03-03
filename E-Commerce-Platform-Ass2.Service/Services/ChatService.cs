using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class ChatService : IChatService
    {
        private readonly IChatSessionRepository _sessionRepository;
        private readonly IChatMessageRepository _messageRepository;

        public ChatService(IChatSessionRepository sessionRepository, IChatMessageRepository messageRepository)
        {
            _sessionRepository = sessionRepository;
            _messageRepository = messageRepository;
        }

        public async Task<ChatSession> GetOrCreateSessionAsync(Guid customerId, Guid shopId)
        {
            var session = await _sessionRepository.GetByCustomerAndShopAsync(customerId, shopId);
            if (session == null)
            {
                session = new ChatSession
                {
                    Id = Guid.NewGuid(),
                    CustomerId = customerId,
                    ShopId = shopId,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await _sessionRepository.CreateAsync(session);
            }
            return session;
        }

        public async Task<IEnumerable<ChatSession>> GetSessionsForCustomerAsync(Guid customerId)
        {
            return await _sessionRepository.GetSessionsByCustomerAsync(customerId);
        }

        public async Task<IEnumerable<ChatSession>> GetSessionsForShopAsync(Guid shopId)
        {
            return await _sessionRepository.GetSessionsByShopAsync(shopId);
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesAsync(Guid sessionId)
        {
            return await _messageRepository.GetMessagesBySessionIdAsync(sessionId);
        }

        public async Task<ChatMessage> SendMessageAsync(Guid sessionId, Guid? senderId, string senderRole, string content, Guid? productId = null)
        {
            var message = new ChatMessage
            {
                Id = Guid.NewGuid(),
                ChatSessionId = sessionId,
                SenderId = senderId,
                SenderRole = senderRole,
                Content = content,
                ProductId = productId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            
            await _messageRepository.CreateAsync(message);

            var session = await _sessionRepository.GetByIdAsync(sessionId);
            if (session != null)
            {
                session.UpdatedAt = DateTime.UtcNow;
                await _sessionRepository.UpdateAsync(session);
            }

            return message;
        }

        public async Task<ChatSession?> GetSessionByIdAsync(Guid sessionId)
        {
            return await _sessionRepository.GetByIdAsync(sessionId);
        }
    }
}
