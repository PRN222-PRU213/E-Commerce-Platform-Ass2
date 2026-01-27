using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.KYC
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IEkycService _eKycService;

        public IndexModel(IEkycService eKycService)
        {
            _eKycService = eKycService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            TempData.Remove("ErrorMessage");

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToPage("/Authentication/Login");
            }

            var isVerified = await _eKycService.IsUserVerifiedAsync(userId);
            if (isVerified)
            {
                return RedirectToPage("/KYC/Status");
            }

            return RedirectToPage("/KYC/Verify");
        }
    }
}

