using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Order
{
    [Authorize]
    public class ReorderModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public ReorderModel(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        public async Task<IActionResult> OnPostAsync(Guid orderId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return RedirectToPage("/Authentication/Login");
            }

            var order = await _orderService.GetOrderItemAsync(orderId);
            if (order != null && order.Status == "Cancelled")
            {
                foreach (var item in order.Items)
                {
                    await _cartService.AddToCart(userId, item.ProductVariantId, item.Quantity);
                }
                TempData["Success"] = "Đã thêm các sản phẩm từ đơn hàng cũ vào giỏ hàng của bạn!";
            }
            else
            {
                TempData["Error"] = "Không thể thực hiện mua lại cho đơn hàng này.";
                return RedirectToPage("/Order/History");
            }

            return RedirectToPage("/Cart/Index");
        }
    }
}
