using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Payment
{
    [Authorize]
    public class PayWithMomoModel : PageModel
    {
        private readonly IMomoService _momoService;
        private readonly ICartService _cartService;
        private readonly IWalletService _walletService;

        public PayWithMomoModel(IMomoService momoService, ICartService cartService, IWalletService walletService)
        {
            _momoService = momoService;
            _cartService = cartService;
            _walletService = walletService;
        }

        [BindProperty]
        public string ShippingAddress { get; set; } = string.Empty;

        [BindProperty]
        public string SelectedCartItemIds { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(SelectedCartItemIds))
            {
                TempData["Error"] = "Vui lòng chọn sản phẩm để thanh toán";
                return RedirectToPage("/Cart/Index");
            }

            var cartItemIds = SelectedCartItemIds
                .Split(',')
                .Select(Guid.Parse)
                .ToList();

            var selectedItems = await _cartService.GetCartItemsByIdsAsync(cartItemIds);

            var totalAmount = selectedItems.Sum(x => x.Quantity * x.ProductVariant.Price);

            if (string.IsNullOrWhiteSpace(ShippingAddress))
            {
                TempData["Error"] = "Vui lòng nhập địa chỉ giao hàng";
                return RedirectToPage("/Cart/Index");
            }

            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var cart = await _cartService.GetCartUserAsync(userId);

            if (cart == null || !cart.Items.Any())
            {
                TempData["Error"] = "Giỏ hàng trống";
                return RedirectToPage("/Cart/Index");
            }

            if (totalAmount <= 0)
            {
                TempData["Error"] = "Số tiền không hợp lệ";
                return RedirectToPage("/Cart/Index");
            }

            // ✅ Lấy số dư ví của người dùng
            var walletDto = await _walletService.GetOrCreateAsync(userId);
            decimal walletBalance = walletDto?.Balance ?? 0;

            decimal walletUsed = 0;
            decimal momoAmount = 0;

            // ✅ Logic thanh toán hybrid
            if (walletBalance >= totalAmount)
            {
                // Trường hợp 1: Ví đủ tiền → chỉ dùng ví
                walletUsed = totalAmount;
                momoAmount = 0;
            }
            else
            {
                // Trường hợp 2: Ví không đủ → dùng hết ví + Momo cho phần còn lại
                walletUsed = walletBalance;
                momoAmount = totalAmount - walletBalance;
            }

            // ✅ Lưu vào Session
            HttpContext.Session.SetString("ShippingAddress", ShippingAddress);
            HttpContext.Session.SetString("SelectedCartItemIds", SelectedCartItemIds);
            HttpContext.Session.SetString("WalletUsed", walletUsed.ToString());
            HttpContext.Session.SetString("MomoAmount", momoAmount.ToString());

            // ✅ Nếu cần thanh toán qua Momo
            if (momoAmount > 0)
            {
                long amount = (long)momoAmount;

                var payUrl = await _momoService.CreatePaymentAsync(
                    amount,
                    "Thanh toán đơn hàng");

                if (string.IsNullOrEmpty(payUrl))
                {
                    TempData["Error"] = "Thanh toán MoMo thất bại";
                    return RedirectToPage("/Cart/Index");
                }

                return Redirect(payUrl);
            }
            else
            {
                // ✅ Nếu chỉ dùng ví (không cần Momo) → chuyển thẳng đến callback
                return RedirectToPage("/Checkout/PaymentCallBack", new
                {
                    resultCode = 0,
                    message = "Thanh toán bằng ví thành công",
                    orderId = Guid.NewGuid().ToString(),
                    transId = ""
                });
            }
        }
    }
}
