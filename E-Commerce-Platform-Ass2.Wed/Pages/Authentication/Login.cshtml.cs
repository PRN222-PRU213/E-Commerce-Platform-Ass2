using System.Collections.Generic;
using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Authentication
{
    public class LoginModel : PageModel
    {
        private readonly IUserService _userService;

        public LoginModel(IUserService userService)
        {
            _userService = userService;
        }

        [BindProperty]
        public LoginViewModel Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        public bool ShowResendLink { get; set; }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;

            // Server-side validation
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Trim and normalize email
            Input.Email = Input.Email.Trim().ToLowerInvariant();

            // Check if email is verified first
            var isVerified = await _userService.IsEmailVerifiedAsync(Input.Email);
            if (!isVerified)
            {
                // Check if user exists (email might not exist at all)
                var user = await _userService.ValidateUserAsync(Input.Email, Input.Password);
                if (user == null)
                {
                    // Could be wrong password OR unverified email
                    // For security, show generic message but also check verification
                    ModelState.AddModelError(string.Empty, "Email chưa được xác thực hoặc thông tin đăng nhập không đúng.");
                    ShowResendLink = true;
                    ViewData["Email"] = Input.Email;
                    return Page();
                }
            }

            var validatedUser = await _userService.ValidateUserAsync(Input.Email, Input.Password);
            if (validatedUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, validatedUser.Id.ToString()),
                new Claim(ClaimTypes.Name, validatedUser.Name),
                new Claim(ClaimTypes.Email, validatedUser.Email),
                new Claim(ClaimTypes.Role, validatedUser.Role),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties { IsPersistent = Input.RememberMe }
            );

            if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
            {
                return Redirect(ReturnUrl);
            }

            // Redirect Admin to Admin Dashboard
            if (validatedUser.Role == "Admin")
            {
                return RedirectToPage("/Admin/Index");
            }

            return RedirectToPage("/Index");
        }
    }
}
