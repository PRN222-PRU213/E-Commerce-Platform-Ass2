using E_Commerce_Platform_Ass1.Data.Database.Entities;

namespace E_Commerce_Platform_Ass1.Data.Repositories.Interfaces
{
    public interface ICartItemRepository
    {
        Task<CartItem?> GetByIdAsync(Guid id);
        Task<CartItem?> GetByCartAndVariantAsync(Guid cartId, Guid productVariantId);
        Task<IEnumerable<CartItem>> GetByCartIdAsync(Guid cartId);
        Task<IEnumerable<CartItem>> GetItemByIdsAsync(IEnumerable<Guid> cartItemIds);
        Task AddAsync(CartItem cartItem);
        Task UpdateAsync(CartItem cartItem);
        Task UpdateQuantityAsync(Guid cartItemId, int quantity);
        Task DeleteAsync(CartItem cartItem);
        Task DeleteByIdsAsync(IEnumerable<Guid> cartItemIds);
    }
}
