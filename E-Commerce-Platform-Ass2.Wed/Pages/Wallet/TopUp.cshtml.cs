using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Wallet
{
    [Authorize]
    public class TopUpModel : PageModel
    {
        private readonly IMomoService _momoService;

        public TopUpModel(IMomoService momoService)
        {
            _momoService = momoService;
        }

        [BindProperty]
        public TopUpViewModel Input { get; set; } = new();

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Validate amount
            if (Input.Amount < 10000)
            {
                ModelState.AddModelError("Input.Amount", "Số tiền nạp tối thiểu là 10,000 VNĐ.");
                return Page();
            }

            if (Input.Amount > 10000000)
            {
                ModelState.AddModelError("Input.Amount", "Số tiền nạp tối đa là 10,000,000 VNĐ.");
                return Page();
            }

            try
            {
                // Lưu số tiền vào session
                HttpContext.Session.SetString("TopUpAmount", Input.Amount.ToString());

                // Tạo payment URL qua Momo
                var payUrl = await _momoService.CreateTopUpPaymentAsync(
                    (long)Input.Amount,
                    $"Nạp tiền ví - {Input.Amount:N0} VNĐ"
                );

                if (string.IsNullOrEmpty(payUrl))
                {
                    TempData["Error"] = "Không thể tạo yêu cầu thanh toán Momo.";
                    return Page();
                }

                return Redirect(payUrl);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
                return Page();
            }
        }
    }
}
