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
        private readonly IRefundService _refundService;

        public HistoryModel(IOrderService orderService, IRefundService refundService)
        {
            _orderService = orderService;
            _refundService = refundService;
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

            // Sequential loop to avoid concurrent DbContext operations (EF Core is not thread-safe)
            var model = new List<OrderHistoryViewModel>();
            foreach (var o in orders)
            {
                model.Add(new OrderHistoryViewModel
                {
                    OrderId = o.Id,
                    OrderDate = o.CreatedAt,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status,
                    ShippingAddress = o.ShippingAddress,
                    IsRefunded = await _refundService.IsRefundedAsync(o.Id)
                });
            }

            model = model.OrderByDescending(o => o.OrderDate).ToList();

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
