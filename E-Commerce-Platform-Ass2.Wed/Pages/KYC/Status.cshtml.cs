using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.KYC
{
    [Authorize]
    public class StatusModel : PageModel
    {
        private readonly IEkycService _eKycService;

        public StatusModel(IEkycService eKycService)
        {
            _eKycService = eKycService;
        }

        public KYCStatusViewModel StatusViewModel { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToPage("/Authentication/Login");
            }

            var isVerified = await _eKycService.IsUserVerifiedAsync(userId);

            StatusViewModel = new KYCStatusViewModel
            {
                IsVerified = isVerified,
                Status = isVerified ? "VERIFIED" : "NOT_VERIFIED",
                Message = isVerified
                    ? "Tài khoản của bạn đã được xác thực danh tính."
                    : "Tài khoản của bạn chưa được xác thực danh tính."
            };

            return Page();
        }
    }
}

