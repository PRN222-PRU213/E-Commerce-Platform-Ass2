using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using System.Security.Claims;
using E_Commerce_Platform_Ass2.Data.Database.Entities;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Api.Chat
{
    public class ShopSessionsModel : PageModel
    {
        private readonly IChatService _chatService;
        private readonly IShopService _shopService;

        public ShopSessionsModel(IChatService chatService, IShopService shopService)
        {
            _chatService = chatService;
            _shopService = shopService;
        }

        public async Task<IActionResult> OnGetAsync(Guid shopId)
        {
            // Verify ownership
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            {
                return Unauthorized();
            }

            var shop = await _shopService.GetShopByIdAsync(shopId);
            if (shop == null || shop.UserId != userId)
            {
                return Unauthorized();
            }

            var sessions = await _chatService.GetSessionsForShopAsync(shopId);

            return new JsonResult(sessions.Select(s => new {
                id = s.Id,
                customerId = s.CustomerId,
                customerName = s.Customer?.Name ?? "Khách hàng",
                updatedAt = s.UpdatedAt.ToString("o"),
                lastMessage = s.Messages?.OrderByDescending(m => m.CreatedAt).FirstOrDefault()?.Content ?? ""
            }));
        }
    }
}
