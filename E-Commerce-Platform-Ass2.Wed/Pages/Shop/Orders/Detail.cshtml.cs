using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop.Orders
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private readonly IShopOrderService _shopOrderService;
        private readonly IShopService _shopService;

        public DetailModel(IShopOrderService shopOrderService, IShopService shopService)
        {
            _shopOrderService = shopOrderService;
            _shopService = shopService;
        }

        public OrderDetailDto Order { get; set; } = new();

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

        private async Task<bool> LoadOrderAsync(Guid id)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return false;
            }

            var result = await _shopOrderService.GetOrderDetailAsync(id, shopId.Value);
            if (!result.IsSuccess || result.Data == null)
            {
                TempData["ErrorMessage"] = result.ErrorMessage;
                return false;
            }

            Order = result.Data;
            return true;
        }

        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            var ok = await LoadOrderAsync(id);
            if (!ok)
            {
                return RedirectToPage("/Shop/Orders/Index");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostStartProcessingAsync(Guid id)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Shop/Orders/Index");
            }

            var result = await _shopOrderService.StartProcessingAsync(id, shopId.Value);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] =
                result.IsSuccess ? "Đã bắt đầu xử lý đơn hàng!" : result.ErrorMessage;

            return RedirectToPage("/Shop/Orders/Detail", new { id });
        }

        public async Task<IActionResult> OnPostStartPreparingAsync(Guid id)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Shop/Orders/Index");
            }

            var result = await _shopOrderService.StartPreparingAsync(id, shopId.Value);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] =
                result.IsSuccess ? "Đã chuyển sang chuẩn bị hàng!" : result.ErrorMessage;

            return RedirectToPage("/Shop/Orders/Detail", new { id });
        }

        public async Task<IActionResult> OnPostShipAsync(Guid id, CreateShipmentDto dto)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Shop/Orders/Index");
            }

            var result = await _shopOrderService.ShipOrderAsync(id, shopId.Value, dto);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] =
                result.IsSuccess
                    ? "Đã gửi hàng thành công! Đơn hàng đang được vận chuyển."
                    : result.ErrorMessage;

            return RedirectToPage("/Shop/Orders/Detail", new { id });
        }

        public async Task<IActionResult> OnPostUpdateShipmentAsync(Guid id, UpdateShipmentDto dto)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Shop/Orders/Index");
            }

            var result = await _shopOrderService.UpdateShipmentAsync(id, shopId.Value, dto);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] =
                result.IsSuccess
                    ? "Đã cập nhật thông tin vận chuyển thành công!"
                    : result.ErrorMessage;

            return RedirectToPage("/Shop/Orders/Detail", new { id });
        }

        public async Task<IActionResult> OnPostMarkDeliveredAsync(Guid id)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Shop/Orders/Index");
            }

            var result = await _shopOrderService.MarkAsDeliveredAsync(id, shopId.Value);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] =
                result.IsSuccess ? "Đã đánh dấu giao hàng thành công!" : result.ErrorMessage;

            return RedirectToPage("/Shop/Orders/Detail", new { id });
        }

        public async Task<IActionResult> OnPostRejectAsync(Guid id, string? reason)
        {
            var shopId = await GetCurrentShopIdAsync();
            if (shopId == null)
            {
                TempData["ErrorMessage"] = "Bạn chưa có shop.";
                return RedirectToPage("/Shop/Orders/Index");
            }

            var result = await _shopOrderService.RejectOrderAsync(id, shopId.Value, reason);
            TempData[result.IsSuccess ? "SuccessMessage" : "ErrorMessage"] =
                result.IsSuccess ? "Đã từ chối đơn hàng!" : result.ErrorMessage;

            return RedirectToPage("/Shop/Orders/Index");
        }
    }
}

