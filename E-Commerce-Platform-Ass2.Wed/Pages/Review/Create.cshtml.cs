using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Review
{
    public class CreateModel : PageModel
    {
        private readonly IReviewService _reviewService;

        public CreateModel(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }
        public void OnGet()
        {
        }
    }
}
