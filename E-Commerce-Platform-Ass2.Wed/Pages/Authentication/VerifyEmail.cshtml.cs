using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Authentication
{
    public class VerifyEmailModel : PageModel
    {
        private readonly IUserService _userService;

        public VerifyEmailModel(IUserService userService)
        {
            _userService = userService;
        }

        public bool Success { get; set; }
        public string? UserName { get; set; }
        public string? ErrorMessage { get; set; }

        public async Task OnGetAsync(string? token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                Success = false;
                ErrorMessage = "Link xác thực không hợp lệ.";
                return;
            }

            var result = await _userService.VerifyEmailAsync(token);

            Success = result.Success;
            UserName = result.UserName;
            ErrorMessage = result.ErrorMessage;
        }
    }
}
