using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Service.DTOs;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetOrderHistoryAsync(Guid userId);
        Task<OrderDetailDto?> GetOrderItemAsync(Guid orderId);
    }
}
