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
    public class CreateModel : PageModel
    {
        private readonly ISupportTicketService _ticketService;
        private readonly IOrderService _orderService;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public CreateModel(
            ISupportTicketService ticketService,
            IOrderService orderService,
            IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _ticketService = ticketService;
            _orderService = orderService;
            _hubContext = hubContext;
        }

        [BindProperty]
        public CreateTicketViewModel Input { get; set; } = new();

        public List<OrderDto> RecentOrders { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var orders = await _orderService.GetOrderHistoryAsync(userId);
            RecentOrders = orders.Take(10).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                var orders = await _orderService.GetOrderHistoryAsync(userId);
                RecentOrders = orders.Take(10).ToList();
                return Page();
            }

            var customerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var customerName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

            var request = new CreateTicketRequest(
                Subject: Input.Subject,
                Description: Input.Description,
                Category: Input.Category,
                Priority: Input.Priority,
                RelatedOrderId: Input.RelatedOrderId,
                RelatedShopId: Input.RelatedShopId
            );

            var result = await _ticketService.CreateTicketAsync(customerId, request);

            try
            {
                var message = new TicketNotificationMessage
                {
                    TicketId = result.Id,
                    TicketCode = result.TicketCode,
                    Subject = result.Subject,
                    Status = result.Status,
                    Category = result.Category,
                    Priority = result.Priority,
                    SenderId = customerId,
                    SenderName = customerName,
                    ShopId = result.RelatedShopId?.ToString()
                };

                if (!string.IsNullOrEmpty(result.RelatedShopId?.ToString()))
                {
                    await _hubContext.Clients.Group($"shop-{result.RelatedShopId}").TicketCreated(message);
                }

                await _hubContext.Clients.Group("admins").TicketCreated(message);
            }
            catch
            {
                // Notification failure should not break ticket creation
            }

            TempData["Success"] = $"Ticket {result.TicketCode} đã được tạo thành công!";
            return RedirectToPage("./Detail", new { id = result.Id });
        }
    }

    public class CreateTicketViewModel
    {
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = "Khác";
        public string Priority { get; set; } = "Medium";
        public Guid? RelatedOrderId { get; set; }
        public Guid? RelatedShopId { get; set; }

        public static List<SelectOption> CategoryOptions => new()
        {
            new SelectOption { Value = "Kỹ thuật", Text = "Kỹ thuật" },
            new SelectOption { Value = "Thanh toán", Text = "Thanh toán" },
            new SelectOption { Value = "Khiếu nại", Text = "Khiếu nại" },
            new SelectOption { Value = "Góp ý", Text = "Góp ý" },
            new SelectOption { Value = "Khác", Text = "Khác" }
        };

        public static List<SelectOption> PriorityOptions => new()
        {
            new SelectOption { Value = "Low", Text = "Thấp" },
            new SelectOption { Value = "Medium", Text = "Trung bình" },
            new SelectOption { Value = "High", Text = "Cao" },
            new SelectOption { Value = "Urgent", Text = "Khẩn cấp" }
        };
    }

    public class SelectOption
    {
        public string Value { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
    }
}
