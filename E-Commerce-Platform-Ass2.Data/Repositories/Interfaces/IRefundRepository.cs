using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass1.Data.Database.Entities;

namespace E_Commerce_Platform_Ass1.Data.Repositories.Interfaces
{
    public interface IRefundRepository
    {
        Task<bool> ExistsRequestIdAsync(string requestId);
        Task AddAsync(Refund refund);
    }
}
