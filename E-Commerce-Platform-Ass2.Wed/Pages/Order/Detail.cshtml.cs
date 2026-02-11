using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Order
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private readonly IOrderService _orderService;

        public DetailModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public OrderDetailViewModel ViewModel { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(Guid orderId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToPage("/Authentication/Login", new { returnUrl = "/" });
            }

            var order = await _orderService.GetOrderItemAsync(orderId);
            Console.WriteLine($"[Customer Detail] Order {orderId} status: {order?.Status}"); // DEBUG

            if (order == null)
                return NotFound();

            ViewModel = new OrderDetailViewModel
            {
                OrderId = order.Id,
                CreatedAt = order.CreatedAt,
                ShippingAddress = order.ShippingAddress,
                Status = order.Status,
                TotalAmount = order.TotalAmount,
                Items = order.Items.Select(o => new OrderItemViewModel
                {
                    ProductName = o.ProductName,
                    ImageUrl = o.ImageUrl ?? string.Empty,
                    Price = o.Price,
                    Quantity = o.Quantity,
                    Size = o.Size,
                    Color = o.Color
                }).ToList()
            };

            return Page();
        }
    }
}
