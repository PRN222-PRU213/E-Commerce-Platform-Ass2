using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Api.Chat
{
    public class SessionModel : PageModel
    {
        private readonly IChatService _chatService;

        public SessionModel(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<IActionResult> OnGetAsync(Guid shopId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr))
            {
                return Unauthorized();
            }

            if (!Guid.TryParse(userIdStr, out var customerId))
            {
                return Unauthorized();
            }

            var session = await _chatService.GetOrCreateSessionAsync(customerId, shopId);
            var messages = await _chatService.GetMessagesAsync(session.Id);

            return new JsonResult(new
            {
                sessionId = session.Id,
                customerId = customerId,
                history = messages.Select(m => new {
                    content = m.Content,
                    senderRole = m.SenderRole,
                    createdAt = m.CreatedAt.ToString("o")
                })
            });
        }
    }
}
