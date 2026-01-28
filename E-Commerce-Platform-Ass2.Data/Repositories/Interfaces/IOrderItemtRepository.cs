using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Data.Database.Entities;

namespace E_Commerce_Platform_Ass1.Data.Repositories.Interfaces
{
    public interface IOrderItemtRepository
    {
        Task<List<OrderItem>> GetByOrderIdAsync(Guid orderId);
        Task AddAsync(OrderItem orderItem);
        Task AddRangeAsync(IEnumerable<OrderItem> orderItems);
        Task DeleteByOrderIdAsync(Guid orderId);
    }
}
