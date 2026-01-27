using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Cart
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ICartService _cartService;

        public IndexModel(ICartService cartService)
        {
            _cartService = cartService;
        }

        public CartViewModel ViewModel { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return RedirectToPage("/Authentication/Login", new { returnUrl = "/Cart" });
            }

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return RedirectToPage("/Authentication/Login");
            }

            var serviceCart = await _cartService.GetCartUserAsync(userId);

            ViewModel = new CartViewModel
            {
                Items = serviceCart
                    ?.Items.Select(i => new CartItemViewModel
                    {
                        CartItemId = i.CartItemId,
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        ImageUrl = i.ImageUrl,
                        Price = i.Price,
                        Quantity = i.Stock, // Service DTO uses Stock for quantity in cart
                        Size = i.Size,
                        Color = i.Color,
                        TotalLinePrice = i.TotalLinePrice,
                    })
                    .ToList() ?? new List<CartItemViewModel>(),
                TotalPrice = serviceCart?.TotalPrice ?? 0,
                Shipping = 0, // Shipping is currently not calculated/required by the user request
            };

            return Page();
        }
    }
}
