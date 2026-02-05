using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Wallet
{
    [Authorize]
    public class TransactionsModel : PageModel
    {
        private readonly IWalletService _walletService;

        public TransactionsModel(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public List<WalletTransactionDto> Transactions { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                return;
            }

            // ⚠️ LƯU Ý: Method GetTransactionsAsync cần được thêm vào IWalletService và WalletService
            Transactions = await _walletService.GetTransactionsAsync(userId, 50);
        }
    }
}
