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

            if (Guid.TryParse(userId, out var userGuid))
            {
                // Nhóm theo user để gửi thông báo cá nhân
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userGuid}");

                // Nhóm theo shop (nếu user có shop)
                var shop = await _shopService.GetShopByUserIdAsync(userGuid);
                if (shop != null)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"shop-{shop.Id}");
                }
            }

            // Nhóm admin chung
            if (
                !string.IsNullOrWhiteSpace(role)
                && role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
            )
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "admins");
            }

            await base.OnConnectedAsync();
        }
    }
}
