using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Admin.Reviews
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IReviewService _reviewService;
        private readonly Microsoft.AspNetCore.SignalR.IHubContext<E_Commerce_Platform_Ass2.Wed.Hubs.NotificationHub, E_Commerce_Platform_Ass2.Wed.Models.SignalR.INotificationClient> _hubContext;

        public IndexModel(
            IReviewService reviewService,
            Microsoft.AspNetCore.SignalR.IHubContext<E_Commerce_Platform_Ass2.Wed.Hubs.NotificationHub, E_Commerce_Platform_Ass2.Wed.Models.SignalR.INotificationClient> hubContext)
        {
            _reviewService = reviewService;
            _hubContext = hubContext;
        }

        public IEnumerable<ReviewDto> AllReviews { get; set; } = new List<ReviewDto>();

        public async Task OnGetAsync()
        {
            AllReviews = (await _reviewService.GetAllAsync()).OrderByDescending(r => r.CreatedAt);
        }

        public async Task<IActionResult> OnPostApproveAsync(Guid id)
        {
            await _reviewService.ApproveReviewAsync(id);
            
            // Broadcast the approved review in real-time
            var allReviews = await _reviewService.GetAllAsync();
            var review = allReviews.FirstOrDefault(r => r.Id == id);
            if (review != null)
            {
                await _hubContext.Clients.All.ReviewApproved(review);
            }

            TempData["Success"] = "Review approved successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRejectAsync(Guid id)
        {
            await _reviewService.RejectReviewAsync(id);
            TempData["Success"] = "Review rejected successfully.";
            return RedirectToPage();
        }
    }
}
