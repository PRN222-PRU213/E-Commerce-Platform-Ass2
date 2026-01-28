using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Data.Database.Entities;

namespace E_Commerce_Platform_Ass1.Data.Repositories.Interfaces
{
    public interface IProductVariantRepository
    {
        Task<IEnumerable<ProductVariant>> GetAllAsync();
        Task<ProductVariant?> GetByIdAsync(Guid id);
        Task<IEnumerable<ProductVariant>> GetByProductIdAsync(Guid productId);
        Task<ProductVariant> AddAsync(ProductVariant productVariant);
        Task<ProductVariant> UpdateAsync(ProductVariant productVariant);
        Task<bool> DeleteAsync(ProductVariant productVariant);
    }
}
