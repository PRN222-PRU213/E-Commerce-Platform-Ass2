using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Notifications
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly INotificationService _notificationService;

        public IndexModel(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnGetJsonAsync(int count = 20)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized();
            }

            var notifications = await _notificationService.GetUserNotificationsAsync(userId, count);
            var unreadCount = await _notificationService.GetUnreadCountAsync(userId);

            return new JsonResult(new { notifications, unreadCount });
        }

        public async Task<IActionResult> OnPostMarkReadAsync(Guid id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> OnPostMarkAllReadAsync()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized();
            }

            await _notificationService.MarkAllAsReadAsync(userId);
            return new JsonResult(new { success = true });
        }
    }
}
