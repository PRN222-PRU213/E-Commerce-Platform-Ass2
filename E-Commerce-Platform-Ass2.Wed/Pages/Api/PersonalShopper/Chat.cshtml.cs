using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Api.PersonalShopper
{
    [IgnoreAntiforgeryToken]
    public class ChatModel : PageModel
    {
        private readonly IPersonalShopperService _shopperService;
        private readonly ILogger<ChatModel> _logger;

        private static readonly JsonSerializerOptions _jsonOptions =
            new() { PropertyNameCaseInsensitive = true };

        public ChatModel(IPersonalShopperService shopperService, ILogger<ChatModel> logger)
        {
            _shopperService = shopperService;
            _logger = logger;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ShopperChatRequest? request = null;
            try
            {
                using var reader = new StreamReader(Request.Body);
                var body = await reader.ReadToEndAsync();
                request = JsonSerializer.Deserialize<ShopperChatRequest>(body, _jsonOptions);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize PersonalShopper request body");
            }

            if (request == null || string.IsNullOrWhiteSpace(request.Message))
                return new JsonResult(new { error = "Message is required." }) { StatusCode = 400 };

            try
            {
                var response = await _shopperService.ChatAsync(request.Message, request.History ?? new());
                return new JsonResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PersonalShopper ChatAsync failed for message: {Message}", request.Message);
                return new JsonResult(new ShopperChatResponse
                {
                    Message = $"⚠️ Xin lỗi, tôi đang gặp sự cố: {ex.Message}",
                    Combos = null
                });
            }
        }
    }
}
