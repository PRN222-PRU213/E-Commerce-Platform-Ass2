using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Wed.Hubs
{
    public class ChatHub(IChatService chatService) : Hub
    {
        private readonly IChatService _chatService = chatService;

        public async Task JoinSession(string sessionId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
        }

        public async Task LeaveSession(string sessionId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
        }

        public async Task JoinShop(string shopId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Shop_{shopId}");
        }

        public async Task LeaveShop(string shopId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Shop_{shopId}");
        }

        public async Task SendMessage(string sessionId, string senderId, string senderRole, string content, string? productId = null)
        {
            var sessionGuid = Guid.Parse(sessionId);
            Guid? parsedSenderId = string.IsNullOrEmpty(senderId) ? null : Guid.Parse(senderId);
            Guid? parsedProductId = string.IsNullOrEmpty(productId) ? null : Guid.Parse(productId);

            var message = await _chatService.SendMessageAsync(sessionGuid, parsedSenderId, senderRole, content, parsedProductId);
            var session = await _chatService.GetSessionByIdAsync(sessionGuid);

            var payload = new
            {
                id = message.Id,
                chatSessionId = message.ChatSessionId.ToString(),
                senderId = message.SenderId,
                senderRole = message.SenderRole,
                content = message.Content,
                productId = message.ProductId,
                createdAt = message.CreatedAt.ToString("o")
            };

            if (session != null)
            {
                await Clients.Groups(sessionId, $"Shop_{session.ShopId}").SendAsync("ReceiveMessage", payload);
            }
            else
            {
                await Clients.Group(sessionId).SendAsync("ReceiveMessage", payload);
            }
        }
    }
}
