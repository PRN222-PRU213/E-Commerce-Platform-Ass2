using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Data.Database.Entities;

namespace E_Commerce_Platform_Ass1.Data.Repositories.Interfaces
{
    public interface IShopRepository
    {
        Task<IEnumerable<Shop>> GetAllAsync();
        Task<Shop?> GetByIdAsync(Guid id);
        Task<Shop?> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Shop>> GetByStatus(string status);
        Task<Shop> AddAsync(Shop shop);
        Task<Shop> UpdateAsync(Shop shop);
        Task<bool> DeleteAsync(Shop shop);
        Task<bool> ExistsByUserId(Guid userId);
        Task<bool> ExistsByShopName(string shopName);
    }
}
