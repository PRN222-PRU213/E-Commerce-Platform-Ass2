using E_Commerce_Platform_Ass2.Data.Database.Entities;

namespace E_Commerce_Platform_Ass2.Data.Repositories.Interfaces
{
    public interface IShopWalletTransactionRepository
    {
        Task AddAsync(ShopWalletTransaction transaction);
        Task<IEnumerable<ShopWalletTransaction>> GetByShopWalletIdAsync(Guid shopWalletId, int limit = 20);
    }
}
