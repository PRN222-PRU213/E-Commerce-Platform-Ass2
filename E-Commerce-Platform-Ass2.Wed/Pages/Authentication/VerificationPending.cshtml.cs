using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Authentication
{
    public class VerificationPendingModel : PageModel
    {
        public string? Email { get; set; }
        public bool EmailSent { get; set; } = true;

        public IActionResult OnGet()
        {
            Email = TempData["Email"]?.ToString();
            EmailSent = TempData["EmailSent"] as bool? ?? true;

            if (string.IsNullOrEmpty(Email))
            {
                return RedirectToPage("/Authentication/Register");
            }

            return Page();
        }
    }
}
