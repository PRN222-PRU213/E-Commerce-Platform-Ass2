using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop.Wallet
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IShopWalletService _shopWalletService;
        private readonly IShopService _shopService;

        public IndexModel(IShopWalletService shopWalletService, IShopService shopService)
        {
            _shopWalletService = shopWalletService;
            _shopService = shopService;
        }

        public ShopWalletViewModel ViewModel { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var shop = await _shopService.GetShopByUserIdAsync(userId);

            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Shop/RegisterShop");
            }

            var wallet = await _shopWalletService.GetOrCreateAsync(shop.Id);
            var transactions = await _shopWalletService.GetTransactionsAsync(shop.Id, 10);

            ViewModel = new ShopWalletViewModel
            {
                ShopId = shop.Id,
                ShopName = shop.ShopName ?? "Shop",
                Balance = wallet.Balance,
                RecentTransactions = transactions
            };

            return Page();
        }
    }
}
