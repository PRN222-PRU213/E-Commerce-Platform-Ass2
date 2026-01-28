using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class MomoApi : IMomoApi
    {
        public Task<MomoRefundResponse> RefundAsync(string transId, decimal amount, string requestId)
        {
            return Task.FromResult(new MomoRefundResponse
            {
                ResultCode = 0,
                Message = "Success",
                RequestId = requestId
            });
        }
    }
}
