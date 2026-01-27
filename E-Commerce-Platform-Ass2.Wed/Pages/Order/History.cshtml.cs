using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Order
{
    [Authorize]
    public class HistoryModel : PageModel
    {
        private readonly IOrderService _orderService;

        public HistoryModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public PagedResult<OrderHistoryViewModel> ViewModel { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int page = 1)
        {
            const int pageSize = 5;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToPage("/Authentication/Login", new { returnUrl = "/" });
            }

            var orders = await _orderService.GetOrderHistoryAsync(userId);

            var model = orders.Select(o => new OrderHistoryViewModel
            {
                OrderId = o.Id,
                OrderDate = o.CreatedAt,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                ShippingAddress = o.ShippingAddress,
                IsRefunded = false
            })
            .OrderByDescending(o => o.OrderDate)
            .ToList();

            ViewModel = new PagedResult<OrderHistoryViewModel>
            {
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = model.Count,
                Items = model
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()
            };

            return Page();
        }
    }
}
