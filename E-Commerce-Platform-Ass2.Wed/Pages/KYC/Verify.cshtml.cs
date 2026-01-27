using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.KYC
{
    [Authorize]
    public class VerifyModel : PageModel
    {
        private readonly IEkycService _eKycService;

        public VerifyModel(IEkycService eKycService)
        {
            _eKycService = eKycService;
        }

        [BindProperty]
        public VerifyViewModel Input { get; set; } = new();

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

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToPage("/Authentication/Login");
            }

            try
            {
                var result = await _eKycService.VerifyAndSaveAsync(
                    userId,
                    Input.FrontCard,
                    Input.BackCard,
                    Input.Selfie
                );

                if (result.IsSuccess)
                {
                    TempData.Remove("ErrorMessage");
                    TempData["SuccessMessage"] = "Xác thực danh tính thành công!";
                    return RedirectToPage("/KYC/Status");
                }

                ModelState.AddModelError(
                    string.Empty,
                    result.Message
                    ?? "Xác thực danh tính thất bại. Vui lòng kiểm tra lại ảnh chụp và thử lại."
                );
                return Page();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Đã xảy ra lỗi trong quá trình xác thực: " + ex.Message
                );
                return Page();
            }
        }
    }
}

