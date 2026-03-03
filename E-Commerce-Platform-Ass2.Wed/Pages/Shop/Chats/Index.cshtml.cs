using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Service.DTOs;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Shop.Chats
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IChatService _chatService;
        private readonly IShopService _shopService;

        public IndexModel(IChatService chatService, IShopService shopService)
        {
            _chatService = chatService;
            _shopService = shopService;
        }

        public Guid UserId { get; set; }
        public ShopDto ViewModel { get; set; } = new ShopDto();
        public IEnumerable<ChatSession> Sessions { get; set; } = new List<ChatSession>();

        public async Task<IActionResult> OnGetAsync()
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
            {
                return RedirectToPage("/Authentication/Login");
            }

            UserId = userId;

            // Get shop owned by this user
            var shop = await _shopService.GetShopByUserIdAsync(userId);
            if (shop == null)
            {
                return RedirectToPage("/Shop/RegisterShop");
            }

            ViewModel.ShopName = shop.ShopName;
            Sessions = await _chatService.GetSessionsForShopAsync(shop.Id);

            return Page();
        }
    }
}
