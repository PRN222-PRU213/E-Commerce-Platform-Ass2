using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using E_Commerce_Platform_Ass2.Data.Database;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Hubs;

namespace E_Commerce_Platform_Ass2.Wed.Infrastructure.BackgroundJobs
{
    public class AiChatFallbackWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AiChatFallbackWorker> _logger;

        public AiChatFallbackWorker(IServiceScopeFactory scopeFactory, ILogger<AiChatFallbackWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessUnansweredChatsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in AiChatFallbackWorker");
                }
                
                // Run every 1 minute
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task ProcessUnansweredChatsAsync(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var geminiService = scope.ServiceProvider.GetRequiredService<IGeminiService>();
            var chatService = scope.ServiceProvider.GetRequiredService<IChatService>();
            var hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<ChatHub>>();

            // 3-minute timeout
            var thresholdTime = DateTime.UtcNow.AddMinutes(-3);

            // Find sessions where the last activity was 3+ minutes ago
            var inactiveSessions = await dbContext.ChatSessions
                .Include(cs => cs.Shop)
                .Where(cs => cs.UpdatedAt <= thresholdTime)
                .ToListAsync(stoppingToken);

            foreach (var session in inactiveSessions)
            {
                // Get the last message of this session
                var lastMessage = await dbContext.ChatMessages
                    .Where(m => m.ChatSessionId == session.Id)
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefaultAsync(stoppingToken);

                // If the last message was from the customer, trigger the AI
                if (lastMessage != null && lastMessage.SenderRole == "Customer")
                {
                    _logger.LogInformation($"Triggering AI fallback for session {session.Id}");

                    // Construct a prompt based on shop's context
                    string prompt = $"You are an AI assistant for the shop '{session.Shop.ShopName}' on an e-commerce platform. " +
                                    $"The customer just said: '{lastMessage.Content}'. " +
                                    $"Please provide a helpful, brief, and polite reply on behalf of the shop since the shop owner is currently busy. Do not make up prices or definitive promises.";

                    try
                    {
                        string aiReply = await geminiService.GenerateContentAsync(prompt);

                        // Save the AI reply as a chat message
                        var message = await chatService.SendMessageAsync(
                            session.Id, 
                            null, // AI doesn't have a specific user ID
                            "AI", 
                            aiReply);

                        // Broadcast to SignalR connected clients
                        await hubContext.Clients.Group(session.Id.ToString()).SendAsync("ReceiveMessage", new
                        {
                            message.Id,
                            message.ChatSessionId,
                            message.SenderId,
                            message.SenderRole,
                            message.Content,
                            CreatedAt = message.CreatedAt.ToString("o")
                        }, stoppingToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Failed to generate or send AI reply for session {session.Id}");
                    }
                }
            }
        }
    }
}
