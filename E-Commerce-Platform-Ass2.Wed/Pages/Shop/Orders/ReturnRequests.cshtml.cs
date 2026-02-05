using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop.Orders
{
    [Authorize]
    public class ReturnRequestsModel : PageModel
    {
        private readonly IShopService _shopService;
        private readonly IReturnRequestService _returnRequestService;

        public ReturnRequestsModel(IShopService shopService, IReturnRequestService returnRequestService)
        {
            _shopService = shopService;
            _returnRequestService = returnRequestService;
        }

        public List<ReturnRequestDto> Requests { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? Status { get; set; }

        private async Task<Guid?> GetCurrentShopIdAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
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

            Requests = (await _returnRequestService.GetShopRequestsAsync(shopId.Value, Status)).ToList();
            ViewData["CurrentStatus"] = Status;
            return Page();
        }
    }
}
