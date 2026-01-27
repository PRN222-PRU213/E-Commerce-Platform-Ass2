using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Cart
{
    [Authorize]
    public class UpdateModel : PageModel
    {
        private readonly ICartService _cartService;

        public UpdateModel(ICartService cartService)
        {
            _cartService = cartService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Quantity { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return new JsonResult(new { success = false, message = "Chưa đăng nhập" }) { StatusCode = 401 };
            }

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return new JsonResult(new { success = false }) { StatusCode = 401 };
            }

            var isUpdated = await _cartService.UpdateQuantityAsync(Id, Quantity);
            if (!isUpdated)
            {
                return new JsonResult(new { message = "Sản phẩm không tồn tại." }) { StatusCode = 404 };
            }

            var updatedItem = await _cartService.GetCartItemAsync(Id);
            var cartTotal = await _cartService.GetCartTotalAsync(userId);

            return new JsonResult(new
            {
                success = true,
                quantity = updatedItem.Quantity,
                lineTotal = updatedItem.Quantity * updatedItem.ProductVariant.Price,
                cartTotal = cartTotal
            });
        }
    }
}
