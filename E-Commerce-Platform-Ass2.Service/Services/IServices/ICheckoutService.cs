using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database.Entities;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface ICheckoutService
    {
        Task<Order> CreateOrderAsync(Guid userId, string shippingAddress, List<Guid> selectedCartItemIds);
        Task<Order> ConfirmPaymentAsync(Guid userId, string shippingAddress, List<Guid> selectedCartItemIds, decimal walletUsed, decimal momoAmount);
    }
}
