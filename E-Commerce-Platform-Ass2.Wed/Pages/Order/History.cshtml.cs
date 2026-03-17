using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Hubs;
using E_Commerce_Platform_Ass2.Wed.Models;
using E_Commerce_Platform_Ass2.Wed.Models.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Order
{
    [Authorize]
    public class HistoryModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IRefundService _refundService;
        private readonly INotificationService _notificationService;
        private readonly IShopService _shopService;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public HistoryModel(
            IOrderService orderService,
            IRefundService refundService,
            INotificationService notificationService,
            IShopService shopService,
            IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _orderService = orderService;
            _refundService = refundService;
            _notificationService = notificationService;
            _shopService = shopService;
            _hubContext = hubContext;
        }

        public PagedResult<OrderHistoryViewModel> ViewModel { get; set; } = new();

        public async Task<IActionResult> OnGetAsync(int pageIndex = 1)
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
                CurrentPage = pageIndex,
                PageSize = pageSize,
                TotalItems = model.Count,
                Items = model
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList()
            };

            return Page();
        }

        public async Task<IActionResult> OnPostConfirmReceivedAsync(Guid orderId, int pageIndex = 1)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToPage("/Authentication/Login", new { returnUrl = "/Order/History" });
            }

            var result = await _orderService.ConfirmReceivedAsync(orderId, userId);
            if (!result.IsSuccess)
            {
                TempData["Error"] = result.ErrorMessage ?? "Không thể xác nhận đã nhận hàng.";
                return RedirectToPage("/Order/History", new { pageIndex });
            }

            TempData["Success"] = "Xác nhận đã nhận hàng thành công.";

            try
            {
                var shopIds = await _orderService.GetShopIdsByOrderAsync(orderId);
                foreach (var shopId in shopIds)
                {
                    var shop = await _shopService.GetShopByIdAsync(shopId);
                    if (shop == null)
                    {
                        continue;
                    }

                    var message = $"Khách hàng đã xác nhận nhận hàng thành công cho đơn #{orderId.ToString()[..8].ToUpper()}.";
                    var link = $"/Shop/Orders/Detail?id={orderId}";

                    await _notificationService.CreateNotificationAsync(shop.UserId, "success", message, link);

                    await _hubContext.Clients.Group($"shop-{shopId}").NotificationReceived(new NotificationMessage
                    {
                        Type = "success",
                        Message = message,
                        Link = link,
                        UserId = userId
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Order History] Notify shop failed after confirm received: {ex.Message}");
            }

            return RedirectToPage("/Order/History", new { pageIndex });
        }
    }
}
