using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Cart
{
    [Authorize]
    public class RemoveModel : PageModel
    {
        private readonly ICartService _cartService;

        public RemoveModel(ICartService cartService)
        {
            _cartService = cartService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return new JsonResult(new { success = false, message = "Chưa đăng nhập" }) { StatusCode = 401 };
                }

                if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
                {
                    return RedirectToPage("/Authentication/Login");
                }

                var isDeleted = await _cartService.RemoveItemAsync(userId, Id);

                if (!isDeleted)
                {
                    return new JsonResult(new { message = "Sản phẩm không tồn tại." }) { StatusCode = 404 };
                }

                var newCartTotal = await _cartService.GetCartTotalAsync(userId);
                var newItemCount = await _cartService.GetTotalItemCountAsync(userId);

                return new JsonResult(new
                {
                    success = true,
                    message = "Đã xóa sản phẩm khỏi giỏ hàng.",
                    newCartTotal = newCartTotal,
                    newItemCount = newItemCount
                });
            }
            catch (UnauthorizedAccessException)
            {
                return new JsonResult(new { success = false, message = "Không có quyền truy cập" }) { StatusCode = 403 };
            }
        }
    }
}
