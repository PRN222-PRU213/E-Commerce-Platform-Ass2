using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Hubs;
using E_Commerce_Platform_Ass2.Wed.Models.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Admin.Tickets
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ISupportTicketService _ticketService;
        private readonly IUserService _userService;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public IndexModel(
            ISupportTicketService ticketService,
            IUserService userService,
            IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _ticketService = ticketService;
            _userService = userService;
            _hubContext = hubContext;
        }

        public List<SupportTicketDto> Tickets { get; set; } = new();
        public TicketStatisticsDto Statistics { get; set; } = new();
        public List<AuthenticatedUser> Admins { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Priority { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Category { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Statistics = await _ticketService.GetStatisticsAsync();
            Tickets = await _ticketService.GetTicketsForAdminAsync(Status, Priority, Category);
            var allUsers = await _userService.GetAllUsersAsync();
            Admins = allUsers.Where(u => u.Role == "Admin").ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAssignAsync(Guid ticketId, Guid? assignedToId)
        {
            var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _ticketService.AssignTicketAsync(ticketId, adminId, assignedToId);

            try
            {
                var ticket = Tickets.FirstOrDefault(t => t.Id == ticketId);
                if (ticket != null)
                {
                    var message = new TicketNotificationMessage
                    {
                        TicketId = ticketId,
                        TicketCode = ticket.TicketCode,
                        Subject = ticket.Subject,
                        Status = "InProgress",
                        Category = ticket.Category,
                        Priority = ticket.Priority,
                        SenderId = adminId,
                        SenderName = User.FindFirstValue(ClaimTypes.Name) ?? "Admin"
                    };
                    await _hubContext.Clients.Group($"user-{ticket.CustomerId}").TicketUpdated(message);
                }
            }
            catch { }

            TempData["Success"] = "Ticket đã được phân công!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(Guid ticketId, string status)
        {
            var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _ticketService.UpdateTicketStatusAsync(ticketId, adminId, "Admin",
                new UpdateTicketStatusRequest(ticketId, status, null, null));

            try
            {
                var ticket = Tickets.FirstOrDefault(t => t.Id == ticketId);
                if (ticket != null)
                {
                    var message = new TicketNotificationMessage
                    {
                        TicketId = ticketId,
                        TicketCode = ticket.TicketCode,
                        Subject = ticket.Subject,
                        Status = status,
                        Category = ticket.Category,
                        Priority = ticket.Priority,
                        SenderId = adminId,
                        SenderName = User.FindFirstValue(ClaimTypes.Name) ?? "Admin"
                    };
                    await _hubContext.Clients.Group($"user-{ticket.CustomerId}").TicketUpdated(message);
                }
            }
            catch { }

            TempData["Success"] = "Trạng thái ticket đã được cập nhật!";
            return RedirectToPage();
        }
    }
}
