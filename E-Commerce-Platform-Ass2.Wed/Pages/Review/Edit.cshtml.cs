using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Service.Services;
using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using E_Commerce_Platform_Ass2.Wed.Hubs;
using E_Commerce_Platform_Ass2.Wed.Models.SignalR;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Review
{
    public class EditModel : PageModel
    {
        private readonly IReviewService _reviewService;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public EditModel(IReviewService reviewService, IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _reviewService = reviewService;
            _hubContext = hubContext;
        }

        [BindProperty]
        public ReviewUpdateModel ReviewUpdate { get; set; } = new();

        public class ReviewUpdateModel
        {
            public Guid Id { get; set; }
            public Guid ProductId { get; set; }
            public string ProductName { get; set; } = string.Empty;
            public int Rating { get; set; }
            public string Comment { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            try
            {
                var review = await _reviewService.GetReviewByIdAsync(id);
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (review == null || userIdStr == null || review.UserId.ToString() != userIdStr)
                {
                    return Unauthorized();
                }

                if (DateTime.Now > review.CreatedAt.AddHours(24))
                {
                    TempData["Error"] = "Bạn chỉ được chỉnh sửa đánh giá trong vòng 24 giờ kể từ khi gửi.";
                    return RedirectToPage("/Product/Detail", new { id = review.ProductId });
                }

                ReviewUpdate = new ReviewUpdateModel
                {
                    Id = review.Id,
                    ProductId = review.ProductId,
                    Rating = review.Rating,
                    Comment = review.Comment,
                    CreatedAt = review.CreatedAt
                };

                return Page();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không tìm thấy bình luận hoặc lỗi hệ thống: " + ex.Message;
                return RedirectToPage("/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized();
            }

            try
            {
                // Verify ownership and time limit again on POST for security
                var existingReview = await _reviewService.GetReviewByIdAsync(ReviewUpdate.Id);
                if (existingReview == null || existingReview.UserId != userId)
                {
                    return Unauthorized();
                }

                if (DateTime.Now > existingReview.CreatedAt.AddHours(24))
                {
                    throw new Exception("Quá thời gian 24 giờ để chỉnh sửa bình luận này.");
                }

                var updatedReview = await _reviewService.UpdateReviewAsync(ReviewUpdate.Id, userId, ReviewUpdate.Rating, ReviewUpdate.Comment);

                // Broadcast to admins for real-time table update (updates existing or adds as pending)
                await _hubContext.Clients.Group("admins").ReviewSubmitted(updatedReview);

                // Notify admins
                await _hubContext.Clients.Group("admins").NotificationReceived(new NotificationMessage
                {
                    Type = "info",
                    Message = $"Một bình luận đã được chỉnh sửa (Rating: {ReviewUpdate.Rating}) chờ duyệt lại.",
                    Link = "/Admin/Reviews/Index",
                    TimestampUtc = DateTime.UtcNow
                });

                TempData["Success"] = "Đánh giá của bạn đã được cập nhật và đang chờ quản trị viên phê duyệt lại.";
                return RedirectToPage("/Product/Detail", new { id = updatedReview.ProductId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return Page();
            }
        }
    }
}
