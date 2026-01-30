using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop.Wallet
{
    [Authorize]
    public class TransactionsModel : PageModel
    {
        private readonly IShopWalletService _shopWalletService;
        private readonly IShopService _shopService;

        public TransactionsModel(IShopWalletService shopWalletService, IShopService shopService)
        {
            _shopWalletService = shopWalletService;
            _shopService = shopService;
        }

        public List<ShopWalletTransactionDto> Transactions { get; set; } = new();
        public string? ShopName { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var shop = await _shopService.GetShopByUserIdAsync(userId);

            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Shop/RegisterShop");
            }

            Transactions = await _shopWalletService.GetTransactionsAsync(shop.Id, 100);
            ShopName = shop.ShopName;

            return Page();
        }
    }
}
