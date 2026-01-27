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
            await _refundService.RefundAsync(OrderId, Amount, Reason);
            TempData["Success"] = "Hoàn tiền thành công";

            return RedirectToPage("/Order/History");
        }
    }
}
