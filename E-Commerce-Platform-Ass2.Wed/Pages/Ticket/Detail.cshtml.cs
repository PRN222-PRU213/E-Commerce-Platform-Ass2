using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Hubs;
using E_Commerce_Platform_Ass2.Wed.Models.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Ticket
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private readonly ISupportTicketService _ticketService;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public DetailModel(
            ISupportTicketService ticketService,
            IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _ticketService = ticketService;
            _hubContext = hubContext;
        }

        public SupportTicketDetailDto? Ticket { get; set; }
        public List<CannedResponseDto> CannedResponses { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public ReplyViewModel Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "Customer";

            Ticket = await _ticketService.GetTicketByIdAsync(Id);

            if (Ticket == null)
            {
                return NotFound();
            }

            if (role != "Admin" && role != "Seller" && Ticket.Ticket.CustomerId != userId)
            {
                return Forbid();
            }

            // Load canned responses for Admin/Seller to use as templates
            if (role == "Admin" || role == "Seller")
            {
                CannedResponses = await _ticketService.GetCannedResponsesAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostReplyAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "Customer";

            Ticket = await _ticketService.GetTicketByIdAsync(Id);

            if (Ticket == null)
            {
                return NotFound();
            }

            if (role != "Admin" && role != "Seller" && Ticket.Ticket.CustomerId != userId)
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(Input.Content))
            {
                TempData["Error"] = "Vui lòng nhập nội dung phản hồi.";
                return Page();
            }

            var replyRequest = new ReplyTicketRequest(Ticket.Ticket.Id, Input.Content, Input.IsInternal);
            var reply = await _ticketService.AddReplyAsync(Ticket.Ticket.Id, userId, role, replyRequest);

            try
            {
                var message = new TicketNotificationMessage
                {
                    TicketId = Ticket.Ticket.Id,
                    TicketCode = Ticket.Ticket.TicketCode,
                    Status = Ticket.Ticket.Status
                };

                if (role == "Customer")
                {
                    if (!string.IsNullOrEmpty(Ticket.Ticket.RelatedShopId?.ToString()))
                    {
                        await _hubContext.Clients.Group($"shop-{Ticket.Ticket.RelatedShopId}").TicketReplied(new TicketReplyNotificationMessage
                        {
                            TicketId = Ticket.Ticket.Id,
                            TicketCode = Ticket.Ticket.TicketCode,
                            ReplyId = reply.Id,
                            SenderName = userName,
                            SenderRole = role,
                            Preview = Input.Content.Length > 100 ? Input.Content[..100] + "..." : Input.Content
                        });
                    }
                    await _hubContext.Clients.Group("admins").TicketReplied(new TicketReplyNotificationMessage
                    {
                        TicketId = Ticket.Ticket.Id,
                        TicketCode = Ticket.Ticket.TicketCode,
                        ReplyId = reply.Id,
                        SenderName = userName,
                        SenderRole = role,
                        Preview = Input.Content.Length > 100 ? Input.Content[..100] + "..." : Input.Content
                    });
                }
                else
                {
                    await _hubContext.Clients.Group($"user-{Ticket.Ticket.CustomerId}").TicketReplied(new TicketReplyNotificationMessage
                    {
                        TicketId = Ticket.Ticket.Id,
                        TicketCode = Ticket.Ticket.TicketCode,
                        ReplyId = reply.Id,
                        SenderName = userName,
                        SenderRole = role,
                        Preview = Input.Content.Length > 100 ? Input.Content[..100] + "..." : Input.Content
                    });
                }
            }
            catch
            {
                // Notification failure should not break reply
            }

            TempData["Success"] = "Phản hồi đã được gửi!";
            return RedirectToPage(new { Id });
        }

        public async Task<IActionResult> OnPostCloseAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = User.FindFirstValue(ClaimTypes.Role) ?? "Customer";

            Ticket = await _ticketService.GetTicketByIdAsync(Id);

            if (Ticket == null)
            {
                return NotFound();
            }

            await _ticketService.CloseTicketAsync(Id, userId, role);

            TempData["Success"] = "Ticket đã được đóng!";
            return RedirectToPage(new { Id });
        }
    }

    public class ReplyViewModel
    {
        public string Content { get; set; } = string.Empty;
        public bool IsInternal { get; set; } = false;
    }
}
