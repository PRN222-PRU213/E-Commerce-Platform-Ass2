using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop
{
    [Authorize]
    public class PreOrdersModel : PageModel
    {
        private readonly IPreOrderService _preOrderService;
        private readonly IShopService _shopService;

        public PreOrdersModel(IPreOrderService preOrderService, IShopService shopService)
        {
            _preOrderService = preOrderService;
            _shopService = shopService;
        }

        public List<PreOrderSummaryDto> Items { get; private set; } = new();

        public Guid CurrentShopId { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!TryGetUserId(out var userId))
                return RedirectToPage("/Authentication/Login");

            var shop = await _shopService.GetShopByUserIdAsync(userId);
            if (shop == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Shop/RegisterShop");
            }

            CurrentShopId = shop.Id;
            Items = await _preOrderService.GetShopPreOrdersAsync(userId);
            return Page();
        }

        public async Task<IActionResult> OnPostMarkReadyAsync(
            Guid preOrderId,
            int deadlineHours = 24
        )
        {
            if (!TryGetUserId(out var userId))
                return RedirectToPage("/Authentication/Login");

            try
            {
                await _preOrderService.MarkReadyForFinalPaymentAsync(
                    userId,
                    new MarkPreOrderReadyRequest
                    {
                        PreOrderId = preOrderId,
                        FinalPaymentDeadlineHours = deadlineHours,
                    }
                );
                TempData["SuccessMessage"] = "Đã mở thanh toán phần còn lại cho khách.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToPage();
        }

        private bool TryGetUserId(out Guid userId)
        {
            userId = Guid.Empty;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out userId);
        }
    }
}
