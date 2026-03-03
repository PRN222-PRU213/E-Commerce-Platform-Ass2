using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Api.Chat
{
    public class HistoryModel : PageModel
    {
        private readonly IChatService _chatService;

        public HistoryModel(IChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task<IActionResult> OnGetAsync(Guid sessionId)
        {
            var session = await _chatService.GetSessionByIdAsync(sessionId);
            if (session == null)
            {
                return NotFound();
            }

            var messages = await _chatService.GetMessagesAsync(sessionId);

            return new JsonResult(messages.Select(m => new {
                content = m.Content,
                senderRole = m.SenderRole,
                createdAt = m.CreatedAt.ToString("o")
            }));
        }
    }
}
