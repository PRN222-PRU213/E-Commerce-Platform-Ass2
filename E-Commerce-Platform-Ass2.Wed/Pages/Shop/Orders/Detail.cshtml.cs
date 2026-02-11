using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Hubs;
using E_Commerce_Platform_Ass2.Wed.Models.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop.Orders
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private readonly IShopOrderService _shopOrderService;
        private readonly IShopService _shopService;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
        private readonly INotificationService _notificationService;

        public DetailModel(
            IShopOrderService shopOrderService,
            IShopService shopService,
            IHubContext<NotificationHub, INotificationClient> hubContext,
            INotificationService notificationService
        )
        {
            _shopOrderService = shopOrderService;
            _shopService = shopService;
            _hubContext = hubContext;
            _notificationService = notificationService;
        }

        public OrderDetailDto Order { get; set; } = new();

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Guid.Empty;
            }

            return userId;
        }

        private async Task<Guid?> GetCurrentShopIdAsync()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return null;
            }

            var shop = await _shopService.GetShopByUserIdAsync(userId);
            return shop?.Id;
        }

        private async Task<bool> LoadOrderAsync(Guid id)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return false;
            }

            var result = await _shopOrderService.GetOrderDetailAsync(id, shopId.Value);
            if (!result.IsSuccess || result.Data == null)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return false;
            }

            Order = result.Data;
            return true;
        }

        /// <summary>
        /// Helper: send a NotificationReceived to the customer who placed the order
        /// </summary>
        private async Task NotifyCustomerAsync(
            Guid orderId,
            Guid customerId,
            string type,
            string message
        )
        {
            Console.WriteLine(
                $"[Shop Order] Notifying customer | CustomerId: {customerId} | OrderId: {orderId} | Type: {type}"
            );

            // 1. Create persistent notification
            var link = $"/Order/Detail?orderId={orderId}";
            if (_notificationService != null)
            {
                await _notificationService.CreateNotificationAsync(customerId, type, message, link);
                Console.WriteLine($"[Shop Order] Persistent notification created");
            }

            // 2. Send Real-time notification
            var notification = new NotificationMessage
            {
                Type = type,
                Message = message,
                Link = link,
                UserId = customerId,
            };

            var groupName = $"user-{customerId}";
            Console.WriteLine($"[Shop Order] Sending SignalR notification to group: {groupName}");

            await _hubContext.Clients.Group(groupName).NotificationReceived(notification);

            Console.WriteLine($"[Shop Order] SignalR notification sent successfully");
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var ok = await LoadOrderAsync(id);
            if (!ok)
            {
                return RedirectToPage("/Shop/Orders/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostStartProcessingAsync(Guid id)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Shop/Orders/Index");
            }

            // Load order to get customer info before action
            var orderResult = await _shopOrderService.GetOrderDetailAsync(id, shopId.Value);

            var result = await _shopOrderService.StartProcessingAsync(id, shopId.Value);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess
                ? "Đã bắt đầu xử lý đơn hàng!"
                : result.ErrorMessage;

            if (result.IsSuccess && orderResult.IsSuccess && orderResult.Data != null)
            {
                await NotifyCustomerAsync(
                    id,
                    orderResult.Data.UserId,
                    "info",
                    $"Đơn hàng #{id.ToString()[..8].ToUpper()} đang được xử lý."
                );
            }

            return RedirectToPage("/Shop/Orders/Detail", new { id });
        }

        public async Task<IActionResult> OnPostStartPreparingAsync(Guid id)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Shop/Orders/Index");
            }

            var orderResult = await _shopOrderService.GetOrderDetailAsync(id, shopId.Value);

            var result = await _shopOrderService.StartPreparingAsync(id, shopId.Value);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess
                ? "Đã chuyển sang chuẩn bị hàng!"
                : result.ErrorMessage;

            if (result.IsSuccess && orderResult.IsSuccess && orderResult.Data != null)
            {
                await NotifyCustomerAsync(
                    id,
                    orderResult.Data.UserId,
                    "info",
                    $"Đơn hàng #{id.ToString()[..8].ToUpper()} đang được chuẩn bị."
                );
            }

            return RedirectToPage("/Shop/Orders/Detail", new { id });
        }

        public async Task<IActionResult> OnPostShipAsync(Guid id, CreateShipmentDto dto)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Shop/Orders/Index");
            }

            var orderResult = await _shopOrderService.GetOrderDetailAsync(id, shopId.Value);

            var result = await _shopOrderService.ShipOrderAsync(id, shopId.Value, dto);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess
                ? "Đã gửi hàng thành công! Đơn hàng đang được vận chuyển."
                : result.ErrorMessage;

            if (result.IsSuccess && orderResult.IsSuccess && orderResult.Data != null)
            {
                await NotifyCustomerAsync(
                    id,
                    orderResult.Data.UserId,
                    "success",
                    $"Đơn hàng #{id.ToString()[..8].ToUpper()} đã được gửi đi! Mã vận đơn: {dto.TrackingCode}"
                );
            }

            return RedirectToPage("/Shop/Orders/Detail", new { id });
        }

        public async Task<IActionResult> OnPostUpdateShipmentAsync(Guid id, UpdateShipmentDto dto)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Shop/Orders/Index");
            }

            var result = await _shopOrderService.UpdateShipmentAsync(id, shopId.Value, dto);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess
                ? "Đã cập nhật thông tin vận chuyển thành công!"
                : result.ErrorMessage;

            return RedirectToPage("/Shop/Orders/Detail", new { id });
        }

        public async Task<IActionResult> OnPostMarkDeliveredAsync(Guid id)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Shop/Orders/Index");
            }

            var orderResult = await _shopOrderService.GetOrderDetailAsync(id, shopId.Value);

            var result = await _shopOrderService.MarkAsDeliveredAsync(id, shopId.Value);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess
                ? "Đã đánh dấu giao hàng thành công!"
                : result.ErrorMessage;

            if (result.IsSuccess && orderResult.IsSuccess && orderResult.Data != null)
            {
                await NotifyCustomerAsync(
                    id,
                    orderResult.Data.UserId,
                    "success",
                    $"Đơn hàng #{id.ToString()[..8].ToUpper()} đã được giao thành công!"
                );
            }

            return RedirectToPage("/Shop/Orders/Detail", new { id });
        }

        public async Task<IActionResult> OnPostRejectAsync(Guid id, string? reason)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Shop/Orders/Index");
            }

            var orderResult = await _shopOrderService.GetOrderDetailAsync(id, shopId.Value);

            var result = await _shopOrderService.RejectOrderAsync(id, shopId.Value, reason);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] = result.IsSuccess
                ? "Đã từ chối đơn hàng!"
                : result.ErrorMessage;

            if (result.IsSuccess && orderResult.IsSuccess && orderResult.Data != null)
            {
                var msg = $"Đơn hàng #{id.ToString()[..8].ToUpper()} đã bị từ chối";
                if (!string.IsNullOrEmpty(reason))
                    msg += $": {reason}";

                await NotifyCustomerAsync(id, orderResult.Data.UserId, "error", msg);
            }

            return RedirectToPage("/Shop/Orders/Index");
        }
    }
}
