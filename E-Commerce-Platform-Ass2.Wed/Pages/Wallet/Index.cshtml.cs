using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Wallet
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IWalletService _walletService;

        public IndexModel(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public WalletViewModel ViewModel { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized();
            }

            var walletDto = await _walletService.GetOrCreateAsync(userId);

            ViewModel = new WalletViewModel
            {
                Balance = walletDto.Balance,
                UpdatedAt = walletDto.UpdatedAt,
                LastChangeAmount = walletDto.LastChangeAmount,
                LastChangeType = walletDto.LastChangeType
            };

            return Page();
        }
    }
}
