using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Hubs;
using E_Commerce_Platform_Ass2.Wed.Models.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Checkout
{
    [Authorize]
    public class PaymentCallBackModel : PageModel
    {
        private readonly ICheckoutService _checkoutService;
        private readonly IUserService _userService;
        private readonly IShopWalletService _shopWalletService;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;
        private readonly IOrderService _orderService;
        private readonly IShopService _shopService;

        public PaymentCallBackModel(
            ICheckoutService checkoutService, 
            IUserService userService,
            IShopWalletService shopWalletService,
            IHubContext<NotificationHub, INotificationClient> hubContext,
            IOrderService orderService,
            IShopService shopService)
        {
            _checkoutService = checkoutService;
            _userService = userService;
            _shopWalletService = shopWalletService;
            _hubContext = hubContext;
            _orderService = orderService;
            _shopService = shopService;
        }

        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public async Task OnGetAsync(int resultCode)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                IsSuccess = false;
                Message = "Không xác định được người dùng.";
                return;
            }

            var user = await _userService.GetUserByIdAsync(userId);
            UserName = user?.Name ?? "Khách hàng";

            if (resultCode == 0)
            {
                var shippingAddress = HttpContext.Session.GetString("ShippingAddress");
                var selectedIdsStr = HttpContext.Session.GetString("SelectedCartItemIds");

                var walletUsedStr = HttpContext.Session.GetString("WalletUsed");
                var momoAmountStr = HttpContext.Session.GetString("MomoAmount");

                if (string.IsNullOrEmpty(shippingAddress) ||
                    string.IsNullOrEmpty(selectedIdsStr) ||
                    string.IsNullOrEmpty(walletUsedStr))
                {
                    IsSuccess = false;
                    Message = "Thiếu thông tin thanh toán.";
                    return;
                }

                var selectedCartItemIds = selectedIdsStr
                    .Split(',')
                    .Select(Guid.Parse)
                    .ToList();

                decimal walletUsed = decimal.Parse(walletUsedStr);
                decimal momoAmount = decimal.Parse(momoAmountStr ?? "0");

                // ⭐ GỌI SERVICE DUY NHẤT
                var newOrder = await _checkoutService.ConfirmPaymentAsync(
                    userId,
                    shippingAddress,
                    selectedCartItemIds,
                    walletUsed,
                    momoAmount
                );

                // ⭐ PHÂN PHỐI TIỀN CHO CÁC SHOP
                await _shopWalletService.DistributeOrderPaymentAsync(newOrder.Id);

                // ⭐ GỬI THÔNG BÁO (PERSISTENT + REAL-TIME)
                try
                {
                    // Injection
                    var notificationService = HttpContext.RequestServices.GetService<INotificationService>();

                    // --- Notify each shop ---
                    var shopIds = await _orderService.GetShopIdsByOrderAsync(newOrder.Id);
                    foreach (var shopId in shopIds)
                    {
                        // Retrieve Shop Owner ID for persistence
                        var shop = await _shopService.GetShopByIdAsync(shopId);
                        if (shop != null && notificationService != null)
                        {
                             await notificationService.CreateNotificationAsync(shop.UserId, "info",
                                $"Đơn hàng mới #{newOrder.Id.ToString()[..8].ToUpper()} - {newOrder.TotalAmount:N0} ₫",
                                "/Shop/Orders/Index");
                        }

                        var shopNotification = new NotificationMessage
                        {
                            Type = "info",
                            Message = $"Đơn hàng mới #{newOrder.Id.ToString()[..8].ToUpper()} - {newOrder.TotalAmount:N0} ₫",
                            Link = "/Shop/Orders/Index",
                            UserId = userId
                        };
                        await _hubContext.Clients.Group($"shop-{shopId}").NotificationReceived(shopNotification);
                    }
                    
                    // 1. Customer Notification
                    if (notificationService != null)
                    {
                        await notificationService.CreateNotificationAsync(userId, "success", 
                            $"Đơn hàng #{newOrder.Id.ToString()[..8].ToUpper()} đặt thành công! Tổng: {newOrder.TotalAmount:N0} ₫", 
                            "/Order/History");
                    }
                    
                    var customerNotification = new NotificationMessage
                    {
                        Type = "success",
                        Message = $"Đơn hàng #{newOrder.Id.ToString()[..8].ToUpper()} đã được đặt thành công! Tổng: {newOrder.TotalAmount:N0} ₫",
                        Link = "/Order/History",
                        UserId = userId
                    };
                    await _hubContext.Clients.Group($"user-{userId}").NotificationReceived(customerNotification);

                    // 2. Admin Notification
                    // (Admins usually check dashboard, persistent notification might spam DB if many admins, but okay)
                    
                    var adminNotification = new NotificationMessage
                    {
                        Type = "info",
                        Message = $"Đơn hàng mới #{newOrder.Id.ToString()[..8].ToUpper()} - {newOrder.TotalAmount:N0} ₫",
                        Link = "/Admin",
                        UserId = userId
                    };
                    await _hubContext.Clients.Group("admins").NotificationReceived(adminNotification);
                }
                catch (Exception ex)
                {
                    // Log error but don't break flow
                    Console.WriteLine($"Notification error: {ex.Message}");
                }

                HttpContext.Session.Clear();

                IsSuccess = true;
                Message =
                    $"Thanh toán thành công!<br/>" +
                    $"Mã đơn hàng: {newOrder.Id}<br/>" +
                    $"Tổng tiền: {newOrder.TotalAmount:N0} VNĐ";
            }
            else
            {
                IsSuccess = false;
                Message = resultCode switch
                {
                    1006 => "Bạn đã hủy thanh toán.",
                    1005 => "Tài khoản không đủ số dư.",
                    1003 => "Giao dịch đã hết hạn.",
                    _ => "Thanh toán không thành công. Vui lòng thử lại."
                };
            }
        }
    }
}
