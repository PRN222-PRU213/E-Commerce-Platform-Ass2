using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Cart
{
    [Authorize]
    public class AddToCartModel : PageModel
    {
        private readonly ICartService _cartService;

        public AddToCartModel(ICartService cartService)
        {
            _cartService = cartService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid ProductId { get; set; }

        [BindProperty]
        public Guid ProductVariantId { get; set; }

        [BindProperty]
        public int Quantity { get; set; } = 1;

        public async Task<IActionResult> OnPostAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToPage("/Authentication/Login", new { returnUrl = $"/Product/Detail?id={ProductId}" });
            }

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return RedirectToPage("/Authentication/Login");
            }

            await _cartService.AddToCart(userId, ProductVariantId, Quantity);

            TempData["SuccessMessage"] = "Đã thêm sản phẩm vào giỏ hàng thành công!";

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return RedirectToPage("/Product/Detail", new { id = ProductId });
        }
    }
}
