using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Wallet
{
    [Authorize]
    public class TopUpCallbackModel : PageModel
    {
        private readonly IWalletService _walletService;

        public TopUpCallbackModel(IWalletService walletService)
        {
            _walletService = walletService;
        }

        public TopUpCallbackViewModel ViewModel { get; set; } = new();

        public async Task OnGetAsync(int resultCode, string? orderId, string? transId)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdClaim, out Guid userId))
            {
                ViewModel.IsSuccess = false;
                ViewModel.Message = "Không xác định được người dùng.";
                return;
            }

            if (resultCode == 0)
            {
                // Thanh toán thành công
                var amountStr = HttpContext.Session.GetString("TopUpAmount");

                if (string.IsNullOrEmpty(amountStr) || !decimal.TryParse(amountStr, out decimal amount))
                {
                    ViewModel.IsSuccess = false;
                    ViewModel.Message = "Không tìm thấy thông tin nạp tiền.";
                    return;
                }

                try
                {
                    // ⚠️ LƯU Ý: Method TopUpAsync cần được thêm vào IWalletService và WalletService
                    var wallet = await _walletService.TopUpAsync(userId, amount, transId);

                    HttpContext.Session.Remove("TopUpAmount");

                    ViewModel.IsSuccess = true;
                    ViewModel.Amount = amount;
                    ViewModel.NewBalance = wallet.Balance;
                    ViewModel.Message = $"Nạp tiền thành công {amount:N0} VNĐ!";
                }
                catch (Exception ex)
                {
                    ViewModel.IsSuccess = false;
                    ViewModel.Message = $"Lỗi khi nạp tiền: {ex.Message}";
                }
            }
            else
            {
                ViewModel.IsSuccess = false;
                ViewModel.Message = resultCode switch
                {
                    1006 => "Bạn đã hủy giao dịch.",
                    1005 => "Tài khoản Momo không đủ số dư.",
                    1003 => "Giao dịch đã hết hạn.",
                    _ => "Thanh toán không thành công. Vui lòng thử lại."
                };
            }
        }
    }
}
