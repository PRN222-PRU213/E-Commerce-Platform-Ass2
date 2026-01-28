using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Service.DTOs;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface IMomoApi
    {
        Task<MomoRefundResponse> RefundAsync(
        string transId,
        decimal amount,
        string requestId);
    }
}
