using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Refund
{
    [Authorize]
    public class RequestModel : PageModel
    {
        private readonly IRefundService _refundService;

        public RequestModel(IRefundService refundService)
        {
            _refundService = refundService;
        }

        [BindProperty]
        public Guid OrderId { get; set; }

        [BindProperty]
        public decimal Amount { get; set; }

        [BindProperty]
        public string Reason { get; set; } = string.Empty;

        public async Task<IActionResult> OnPostAsync()
        {
            // Basic validation
            if (OrderId == Guid.Empty || Amount <= 0 || string.IsNullOrWhiteSpace(Reason))
            {
                TempData["Error"] = "Vui lòng điền đầy đủ thông tin hoàn tiền.";
                return RedirectToPage("/Order/History");
            }

            try
            {
                // Guard: already refunded?
                var alreadyRefunded = await _refundService.IsRefundedAsync(OrderId);
                if (alreadyRefunded)
                {
                    TempData["Error"] = "Đơn hàng này đã được hoàn tiền trước đó.";
                    return RedirectToPage("/Order/History");
                }

                await _refundService.RefundAsync(OrderId, Amount, Reason);
                TempData["Success"] = "Hoàn tiền thành công! Số tiền đã được cộng vào ví của bạn.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message switch
                {
                    "Order not found."        => "Không tìm thấy đơn hàng.",
                    "Payment not found."      => "Không tìm thấy thông tin thanh toán.",
                    "Payment not refundable"  => "Đơn hàng không thể hoàn tiền (đã hoàn hoặc chưa thanh toán).",
                    "Duplicate refund request"=> "Yêu cầu hoàn tiền bị trùng lặp.",
                    "MoMo refund failed"      => "Hoàn tiền qua MoMo thất bại. Vui lòng liên hệ hỗ trợ.",
                    _                         => $"Hoàn tiền thất bại: {ex.Message}"
                };
            }

            return RedirectToPage("/Order/History");
        }
    }
}
