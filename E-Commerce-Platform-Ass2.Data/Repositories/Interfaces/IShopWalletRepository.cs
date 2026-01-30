using E_Commerce_Platform_Ass2.Data.Database.Entities;

namespace E_Commerce_Platform_Ass2.Data.Repositories.Interfaces
{
    public interface IShopWalletRepository
    {
        Task<ShopWallet?> GetByShopIdAsync(Guid shopId);
        Task<ShopWallet> GetOrCreateAsync(Guid shopId);
        Task UpdateAsync(ShopWallet wallet);
    }
}
