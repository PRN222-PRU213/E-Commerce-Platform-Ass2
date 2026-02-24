using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database.Entities;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface IRefundService
    {
        Task RefundAsync(Guid orderId, decimal amount, string reason);

        Task<bool> IsRefundedAsync(Guid orderId);
    }
}
