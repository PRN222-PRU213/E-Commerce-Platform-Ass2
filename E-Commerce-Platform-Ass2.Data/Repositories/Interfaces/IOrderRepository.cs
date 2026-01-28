using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Data.Database.Entities;

namespace E_Commerce_Platform_Ass1.Data.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order?> GetByIdAsync(Guid orderId);
        Task<Order?> GetByIdWithItemsAsync(Guid orderId);
        Task<Order?> GetByIdWithDetailsAsync(Guid orderId);
        Task<IEnumerable<Order>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Order>> GetByShopIdAsync(Guid shopId);
        Task<IEnumerable<Order>> GetAllAsync();
        Task<IEnumerable<Order>> GetAllWithDetailsAsync();
        Task<Order> AddAsync(Order order);
        Task<Order> UpdateAsync(Order order);
    }
}
