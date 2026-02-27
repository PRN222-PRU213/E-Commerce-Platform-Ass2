using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Hubs;
using E_Commerce_Platform_Ass2.Wed.Models.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Review
{
    public class CreateModel : PageModel
    {
        private readonly IReviewService _reviewService;
        private readonly IOrderService _orderService;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public CreateModel(
            IReviewService reviewService, 
            IOrderService orderService,
            IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _reviewService = reviewService;
            _orderService = orderService;
            _hubContext = hubContext;
        }

        public async Task<IActionResult> OnPostAsync(Guid productId, int rating, string comment, IFormFile? reviewImage)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized();
            }

            var exists = await _orderService.ExistsOrderAsync(userId, productId);
            if (!exists)
            {
                TempData["Error"] = "Bạn chưa mua sản phẩm này, nên không thể bình luận.";
                return RedirectToPage("/Product/Detail", new { id = productId });
            }

            try
            {
                var review = await _reviewService.CreateReviewAsync(userId, productId, rating, comment, reviewImage);
                
                // Broadcast to admins for real-time table update
                await _hubContext.Clients.Group("admins").ReviewSubmitted(review);

                // Notify admins (for the bell icon)
                await _hubContext.Clients.Group("admins").NotificationReceived(new E_Commerce_Platform_Ass2.Wed.Models.SignalR.NotificationMessage
                {
                    Type = "info",
                    Message = $"Có bình luận mới cho sản phẩm (Rating: {rating}) chờ duyệt.",
                    Link = "/Admin/Reviews/Index",
                    TimestampUtc = DateTime.UtcNow
                });

                TempData["Success"] = "Đánh giá của bạn đã được gửi và đang chờ quản trị viên phê duyệt.";
                return RedirectToPage("/Product/Detail", new { id = productId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToPage("/Product/Detail", new { id = productId });
            }
        }
    }
}
