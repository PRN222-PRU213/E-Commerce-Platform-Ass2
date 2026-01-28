using E_Commerce_Platform_Ass1.Data.Database.Entities;

namespace E_Commerce_Platform_Ass1.Data.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(Guid id);
        Task<IEnumerable<Product>> GetByShopIdAsync(Guid shopId);
        Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId);
        Task<Product?> GetProductWithVariantsAsync(Guid id);
        Task<bool> ExistsAsync(Guid productId);
        Task<Product> AddAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<bool> DeleteAsync(Product product);
    }
}
