using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.PreOrder
{
    [Authorize]
    public class MyOrdersModel : PageModel
    {
        private readonly IPreOrderService _preOrderService;

        public MyOrdersModel(IPreOrderService preOrderService)
        {
            _preOrderService = preOrderService;
        }

        public List<PreOrderSummaryDto> Items { get; private set; } = new();

        public Guid CurrentUserId { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!TryGetUserId(out var userId))
                return RedirectToPage("/Authentication/Login");

            CurrentUserId = userId;
            Items = await _preOrderService.GetMyPreOrdersAsync(userId);
            return Page();
        }

        public async Task<IActionResult> OnPostPayDepositAsync(
            Guid preOrderId,
            string paymentMethod = "WALLET"
        )
        {
            if (!TryGetUserId(out var userId))
                return RedirectToPage("/Authentication/Login");

            try
            {
                await _preOrderService.PayDepositAsync(
                    userId,
                    new PayPreOrderDepositRequest
                    {
                        PreOrderId = preOrderId,
                        PaymentMethod = paymentMethod,
                    }
                );
                TempData["SuccessMessage"] = "Thanh toán cọc thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostPayRemainingAsync(
            Guid preOrderId,
            string paymentMethod = "WALLET"
        )
        {
            if (!TryGetUserId(out var userId))
                return RedirectToPage("/Authentication/Login");

            try
            {
                await _preOrderService.PayRemainingAsync(
                    userId,
                    new FinalizePreOrderPaymentRequest
                    {
                        PreOrderId = preOrderId,
                        PaymentMethod = paymentMethod,
                    }
                );
                TempData["SuccessMessage"] = "Thanh toán phần còn lại thành công.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToPage();
        }

        private bool TryGetUserId(out Guid userId)
        {
            userId = Guid.Empty;
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out userId);
        }
    }
}
