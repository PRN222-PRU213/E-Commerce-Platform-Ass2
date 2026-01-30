using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Models;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface IReturnRequestService
    {
        // Customer
        Task<ServiceResult<ReturnRequestDto>> CreateRequestAsync(CreateReturnRequestDto dto);
        Task<IEnumerable<ReturnRequestDto>> GetMyRequestsAsync(Guid userId);
        Task<ReturnRequestDto?> GetRequestDetailAsync(Guid requestId, Guid userId);
        Task<bool> CanCreateRequestAsync(Guid orderId, Guid userId);

        // Shop
        Task<IEnumerable<ReturnRequestDto>> GetShopRequestsAsync(Guid shopId, string? status = null);
        Task<ReturnRequestDto?> GetShopRequestDetailAsync(Guid requestId, Guid shopId);
        Task<ServiceResult> ApproveRequestAsync(Guid requestId, Guid shopId, decimal? approvedAmount, string? response);
        Task<ServiceResult> RejectRequestAsync(Guid requestId, Guid shopId, string response);
    }
}
