using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop.Orders
{
    [Authorize]
    public class ReturnRequestDetailModel : PageModel
    {
        private readonly IShopService _shopService;
        private readonly IReturnRequestService _returnRequestService;

        public ReturnRequestDetailModel(IShopService shopService, IReturnRequestService returnRequestService)
        {
            _shopService = shopService;
            _returnRequestService = returnRequestService;
        }

        public ReturnRequestDto Request { get; set; } = new();

        [BindProperty]
        public decimal? ApprovedAmount { get; set; }

        [BindProperty]
        public string? Response { get; set; }

        [BindProperty]
        public string? RejectReason { get; set; }

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

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Shop/RegisterShop");
            }

            Request = await _returnRequestService.GetShopRequestDetailAsync(id, shopId.Value);

            if (Request == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy yêu cầu này.";
                return RedirectToPage("./ReturnRequests");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostApproveReturnAsync(Guid id)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("./ReturnRequests");
            }

            var result = await _returnRequestService.ApproveRequestAsync(id, shopId.Value, ApprovedAmount, Response);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Đã duyệt yêu cầu và hoàn tiền thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }

            return RedirectToPage("./ReturnRequestDetail", new { id });
        }

        public async Task<IActionResult> OnPostRejectReturnAsync(Guid id)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("./ReturnRequests");
            }

            var result = await _returnRequestService.RejectRequestAsync(id, shopId.Value, RejectReason);

            if (result.IsSuccess)
            {
                TempData["SuccessMessage"] = "Đã từ chối yêu cầu hoàn trả.";
            }
            else
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
            }

            return RedirectToPage("./ReturnRequestDetail", new { id });
        }
    }
}
