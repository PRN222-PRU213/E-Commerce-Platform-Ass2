using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace E_Commerce_Platform_Ass2.Wed.Hubs
{
    /// <summary>
    /// Hub push thông báo real-time cho client (sản phẩm, thông báo hệ thống).
    /// </summary>
    [Authorize]
    public class NotificationHub : Hub<INotificationClient>
    {
        private readonly IShopService _shopService;

        public NotificationHub(IShopService shopService)
        {
            _shopService = shopService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var role = Context.User?.FindFirstValue(ClaimTypes.Role);

            Console.WriteLine(
                $"[SignalR Hub] User connected | UserId: {userId} | Role: {role} | ConnectionId: {Context.ConnectionId}"
            );

            if (Guid.TryParse(userId, out var userGuid))
            {
                // Nhóm theo user để gửi thông báo cá nhân
                var userGroup = $"user-{userGuid}";
                await Groups.AddToGroupAsync(Context.ConnectionId, userGroup);
                Console.WriteLine($"[SignalR Hub] Added to group: {userGroup}");

                // Nhóm theo shop (nếu user có shop)
                var shop = await _shopService.GetShopByUserIdAsync(userGuid);
                if (shop != null)
                {
                    var shopGroup = $"shop-{shop.Id}";
                    await Groups.AddToGroupAsync(Context.ConnectionId, shopGroup);
                    Console.WriteLine($"[SignalR Hub] Added to shop group: {shopGroup}");
                }
            }

            // Nhóm admin chung
            if (
                !string.IsNullOrWhiteSpace(role)
                && role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
            )
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "admins");
                Console.WriteLine($"[SignalR Hub] Added to admins group");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine(
                $"[SignalR Hub] User disconnected | UserId: {userId} | ConnectionId: {Context.ConnectionId}"
            );
            if (exception != null)
            {
                Console.WriteLine($"[SignalR Hub] Disconnect exception: {exception.Message}");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
