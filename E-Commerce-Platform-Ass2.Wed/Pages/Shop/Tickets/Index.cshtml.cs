using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop.Tickets
{
    [Authorize(Roles = "Seller")]
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
        public Guid ShopId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var shop = await _shopService.GetShopByUserIdAsync(userId);

            if (shop == null)
            {
                TempData["Error"] = "Bạn chưa có shop.";
                return RedirectToPage("/Index");
            }

            ShopId = shop.Id;

            var allTickets = await _ticketService.GetTicketsForShopAsync(ShopId);

            if (!string.IsNullOrEmpty(Status) && Status != "All")
            {
                Tickets = allTickets.FindAll(t => t.Status == Status);
            }
            else
            {
                Tickets = allTickets;
            }

            return Page();
        }
    }
}
