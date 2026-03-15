using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Api.PersonalShopper
{
    [Authorize]
    [IgnoreAntiforgeryToken]
    public class AddComboModel : PageModel
    {
        private readonly IPersonalShopperService _shopperService;

        public AddComboModel(IPersonalShopperService shopperService)
        {
            _shopperService = shopperService;
        }

        public async Task<IActionResult> OnPostAsync([FromBody] AddComboToCartRequest request)
        {
            if (request == null || request.VariantIds == null || !request.VariantIds.Any())
                return BadRequest(new { error = "VariantIds is required." });

            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var result = await _shopperService.AddComboToCartAsync(userId, request.VariantIds);
            if (result.AddedCount == 0)
                return BadRequest(
                    new
                    {
                        success = false,
                        error = result.Message,
                        result,
                    }
                );

            return new JsonResult(
                new
                {
                    success = true,
                    count = result.AddedCount,
                    result,
                }
            );
        }
    }
}
