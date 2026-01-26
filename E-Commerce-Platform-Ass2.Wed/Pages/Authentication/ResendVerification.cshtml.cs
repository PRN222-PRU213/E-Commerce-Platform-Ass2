using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Authentication
{
    public class ResendVerificationModel : PageModel
    {
        private readonly IUserService _userService;

        public ResendVerificationModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty(SupportsGet = true)]
        public string? Email { get; set; }

        [BindProperty]
        public string? InputEmail { get; set; }

        public void OnGet(string? email = null)
        {
            Email = email;
            InputEmail = email;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var email = InputEmail;
            if (string.IsNullOrWhiteSpace(email))
            {
                ModelState.AddModelError("InputEmail", "Vui lòng nhập email.");
                return Page();
            }

            email = email.Trim().ToLowerInvariant();
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            var result = await _userService.ResendVerificationEmailAsync(email, baseUrl);

            if (!result.Success)
            {
                ModelState.AddModelError("InputEmail", result.ErrorMessage ?? "Đã có lỗi xảy ra.");
                return Page();
            }

            TempData["SuccessMessage"] = "Email xác thực đã được gửi! Vui lòng kiểm tra hộp thư của bạn.";
            return RedirectToPage("/Authentication/Login");
        }
    }
}
