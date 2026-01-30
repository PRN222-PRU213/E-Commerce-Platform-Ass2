using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.ReturnRequest
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private readonly IReturnRequestService _returnRequestService;

        public DetailModel(IReturnRequestService returnRequestService)
        {
            _returnRequestService = returnRequestService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public ReturnRequestDto? Request { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            Request = await _returnRequestService.GetRequestDetailAsync(Id, userId);
            if (Request == null) return NotFound();

            return Page();
        }
    }
}
