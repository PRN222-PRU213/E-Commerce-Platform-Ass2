using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Service.DTOs;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface ICartService
    {
        Task AddToCart(Guid userId, Guid productVariantId, int quantity);

        Task<CartViewModel?> GetCartUserAsync(Guid userId);

        Task<decimal> GetCartTotalAsync(Guid userId);

        Task<int> GetTotalItemCountAsync(Guid userId);

        Task<bool> RemoveItemAsync(Guid userId, Guid cartItemId);
        Task<bool> UpdateQuantityAsync(Guid cartItemId, int quantity);
        Task<CartItem> GetCartItemAsync(Guid cartItemId);
        Task<IEnumerable<CartItem>> GetCartItemsByIdsAsync(IEnumerable<Guid> cartItemIds);
    }
}
