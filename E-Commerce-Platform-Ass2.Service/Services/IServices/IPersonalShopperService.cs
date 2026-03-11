using E_Commerce_Platform_Ass2.Service.DTOs;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface IPersonalShopperService
    {
        Task<ShopperChatResponse> ChatAsync(string userMessage, List<ShopperMessageDto> history);
        Task AddComboToCartAsync(Guid userId, List<Guid> variantIds);
    }
}
