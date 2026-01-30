using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.ReturnRequest
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IReturnRequestService _returnRequestService;

        public IndexModel(IReturnRequestService returnRequestService)
        {
            _returnRequestService = returnRequestService;
        }

        public List<ReturnRequestDto> Requests { get; set; } = new();

        public async Task OnGetAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            Requests = (await _returnRequestService.GetMyRequestsAsync(userId)).ToList();
        }
    }
}
