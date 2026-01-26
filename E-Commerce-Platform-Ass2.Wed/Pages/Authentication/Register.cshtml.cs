using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Authentication
{
    public class RegisterModel : PageModel
    {
        private readonly IUserService _userService;

        public RegisterModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public RegisterViewModel Input { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Server-side validation
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Trim whitespace from inputs
            Input.Name = Input.Name.Trim();
            Input.Email = Input.Email.Trim().ToLowerInvariant();

            // Get base URL for verification link
            var baseUrl = $"{Request.Scheme}://{Request.Host}";

            // Register with email verification
            var result = await _userService.RegisterWithVerificationAsync(Input.Name, Input.Email, Input.Password, baseUrl);
            
            if (!result.Success)
            {
                ModelState.AddModelError(nameof(Input.Email), result.ErrorMessage ?? "Đã có lỗi xảy ra.");
                return Page();
            }

            // Redirect to verification pending page
            TempData["Email"] = Input.Email;
            TempData["EmailSent"] = result.EmailSent;
            return RedirectToPage("/Authentication/VerificationPending");
        }
    }
}
