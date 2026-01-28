using E_Commerce_Platform_Ass1.Data.Database.Entities;

namespace E_Commerce_Platform_Ass1.Data.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetByIdAsync(Guid cartId);
        Task<IEnumerable<Cart>> GetByUserIdAsync(Guid userId);
        Task<Cart?> GetCartByUserIdAsync(Guid userId);
        Task<decimal> GetCartTotalAsync(Guid userId);
        Task<int> GetTotalItemCountAsync(Guid userId);
        Task<Cart> CreateAsync(Cart cart);
        Task<Cart> UpdateAsync(Cart cart);
        Task<bool> DeleteAsync(Cart cart);
    }
}
