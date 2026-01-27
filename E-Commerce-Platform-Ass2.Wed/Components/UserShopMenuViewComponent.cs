using System;
using System.Security.Claims;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce_Platform_Ass2.Wed.Components
{
    public class UserShopMenuViewComponent : ViewComponent
    {
        private readonly IShopService _shopService;

        public UserShopMenuViewComponent(IShopService shopService)
        {
            _shopService = shopService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!ViewContext.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                return Content(string.Empty);
            }

            var userIdClaim = ViewContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Content(string.Empty);
            }

            var hasShop = await _shopService.UserHasShopAsync(userId);
            return View(hasShop);
        }
    }
}

