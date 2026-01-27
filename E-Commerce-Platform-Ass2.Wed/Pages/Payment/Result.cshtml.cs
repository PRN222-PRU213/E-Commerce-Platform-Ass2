using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Payment
{
    public class ResultModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? ResultCode { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Message { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? OrderId { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? Amount { get; set; }

        public bool IsSuccess => ResultCode == "0";

        public void OnGet()
        {
            // Read from query string if not bound
            if (string.IsNullOrEmpty(ResultCode))
            {
                ResultCode = Request.Query["resultCode"].ToString();
            }
            if (string.IsNullOrEmpty(Message))
            {
                Message = Request.Query["message"].ToString();
            }
            if (string.IsNullOrEmpty(OrderId))
            {
                OrderId = Request.Query["orderId"].ToString();
            }
            if (string.IsNullOrEmpty(Amount))
            {
                Amount = Request.Query["amount"].ToString();
            }
        }
    }
}
