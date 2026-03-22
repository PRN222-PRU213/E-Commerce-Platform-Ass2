using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Ticket
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ISupportTicketService _ticketService;
        private readonly IShopService _shopService;

        public IndexModel(ISupportTicketService ticketService, IShopService shopService)
        {
            _ticketService = ticketService;
            _shopService = shopService;
        }

        public List<SupportTicketDto> Tickets { get; set; } = new();
        public List<SupportTicketDto> AllTickets { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? StatusFilter { get; set; }

        public async Task OnGetAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            AllTickets = await _ticketService.GetMyTicketsAsync(userId);

            if (!string.IsNullOrEmpty(StatusFilter) && StatusFilter != "All")
            {
                Tickets = AllTickets.FindAll(t => t.Status == StatusFilter);
            }
            else
            {
                Tickets = AllTickets;
            }
        }
    }
}
