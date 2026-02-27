using System.Security.Claims;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Review
{
    public class DeleteModel : PageModel
    {
        private readonly IReviewService _reviewService;

        public DeleteModel(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        public async Task<IActionResult> OnPostDeleteAsync(Guid reviewId)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                return Unauthorized();
            }

            var review = await _reviewService.GetReviewByIdAsync(reviewId);
            var productId = review.ProductId;

            try
            {
                if (DateTime.Now > review.CreatedAt.AddHours(24))
                {
                    TempData["Error"] = "Bạn chỉ được xóa đánh giá trong vòng 24 giờ kể từ khi gửi.";
                    return RedirectToPage("/Product/Detail", new { id = productId });
                }

                await _reviewService.DeleteReviewAsync(reviewId, userId);
                TempData["Success"] = "Xóa bình luận thành công";
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
