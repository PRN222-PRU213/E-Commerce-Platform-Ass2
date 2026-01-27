using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Checkout
{
    [Authorize]
    public class PaymentCallBackModel : PageModel
    {
        private readonly ICheckoutService _checkoutService;
        private readonly IUserService _userService;

        public PaymentCallBackModel(ICheckoutService checkoutService, IUserService userService)
        {
            _checkoutService = checkoutService;
            _userService = userService;
        }

        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;

        public async Task OnGetAsync(int resultCode)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                IsSuccess = false;
                Message = "Không xác định được người dùng.";
                return;
            }

            var user = await _userService.GetUserByIdAsync(userId);
            UserName = user.Name;

            if (resultCode == 0)
            {
                var shippingAddress = HttpContext.Session.GetString("ShippingAddress");
                var selectedIdsStr = HttpContext.Session.GetString("SelectedCartItemIds");

                var walletUsedStr = HttpContext.Session.GetString("WalletUsed");
                var momoAmountStr = HttpContext.Session.GetString("MomoAmount");

                if (string.IsNullOrEmpty(shippingAddress) ||
                    string.IsNullOrEmpty(selectedIdsStr) ||
                    string.IsNullOrEmpty(walletUsedStr))
                {
                    IsSuccess = false;
                    Message = "Thiếu thông tin thanh toán.";
                    return;
                }

                var selectedCartItemIds = selectedIdsStr
                    .Split(',')
                    .Select(Guid.Parse)
                    .ToList();

                decimal walletUsed = decimal.Parse(walletUsedStr);
                decimal momoAmount = decimal.Parse(momoAmountStr ?? "0");

                // ⭐ GỌI SERVICE DUY NHẤT
                var newOrder = await _checkoutService.ConfirmPaymentAsync(
                    userId,
                    shippingAddress,
                    selectedCartItemIds,
                    walletUsed,
                    momoAmount
                );

                HttpContext.Session.Clear();

                IsSuccess = true;
                Message =
                    $"Thanh toán thành công!<br/>" +
                    $"Mã đơn hàng: {newOrder.Id}<br/>" +
                    $"Tổng tiền: {newOrder.TotalAmount:N0} VNĐ";
            }
            else
            {
                IsSuccess = false;
                Message = resultCode switch
                {
                    1006 => "Bạn đã hủy thanh toán.",
                    1005 => "Tài khoản không đủ số dư.",
                    1003 => "Giao dịch đã hết hạn.",
                    _ => "Thanh toán không thành công. Vui lòng thử lại."
                };
            }
        }
    }
}
