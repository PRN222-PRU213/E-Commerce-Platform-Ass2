using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Authentication
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IWalletService _walletService;

        public ProfileModel(IUserService userService, IWalletService walletService)
        {
            _userService = userService;
            _walletService = walletService;
        }

        public Service.Services.AuthenticatedUser? UserInfo { get; set; }
        public WalletDto? AdminWallet { get; set; }
        public List<WalletTransactionDto> AdminCommissionTransactions { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return RedirectToPage("/Authentication/Login");
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToPage("/Authentication/Login");
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return RedirectToPage("/Authentication/Login");
            }

            UserInfo = user;

            if (string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
            {
                var wallet = await _walletService.GetOrCreateAsync(userId);
                var transactions = await _walletService.GetTransactionsAsync(userId, 20);

                AdminWallet = wallet;
                AdminCommissionTransactions = transactions
                    .Where(t => string.Equals(t.TransactionType, "RefundCommission", StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            return Page();
        }
    }
}
