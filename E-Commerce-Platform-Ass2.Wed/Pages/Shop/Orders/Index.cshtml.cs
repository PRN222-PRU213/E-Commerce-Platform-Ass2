using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop.Orders
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IShopOrderService _shopOrderService;
        private readonly IShopService _shopService;

        public IndexModel(IShopOrderService shopOrderService, IShopService shopService)
        {
            _shopOrderService = shopOrderService;
            _shopService = shopService;
        }

        public List<OrderDto> Orders { get; set; } = new();
        public ShopOrderStatistics Statistics { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Guid.Empty;
            }

            return userId;
        }

        private async Task<Guid?> GetCurrentShopIdAsync()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
            {
                return null;
            }

            var shop = await _shopService.GetShopByUserIdAsync(userId);
            return shop?.Id;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Shop/RegisterShop");
            }

            var result = await _shopOrderService.GetOrdersByShopIdAsync(shopId.Value);
            if (!result.IsSuccess)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                Orders = new List<OrderDto>();
            }
            else
            {
                var orders = result.Data ?? new List<OrderDto>();
                if (!string.IsNullOrEmpty(Status))
                {
                    Orders = orders.Where(o => o.Status == Status).ToList();
                }
                else
                {
                    Orders = orders.ToList();
                }
            }

            Statistics = await _shopOrderService.GetOrderStatisticsAsync(shopId.Value);
            return Page();
        }
    }
}

