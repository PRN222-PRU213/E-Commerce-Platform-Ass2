using E_Commerce_Platform_Ass2.Service.DTOs;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface IPreOrderService
    {
        Task<PreOrderStatusDto> CreateAsync(Guid userId, CreatePreOrderRequest request);
        Task<PreOrderStatusDto> PayDepositAsync(Guid userId, PayPreOrderDepositRequest request);
        Task<PreOrderStatusDto> MarkReadyForFinalPaymentAsync(
            Guid shopUserId,
            MarkPreOrderReadyRequest request
        );
        Task<PreOrderStatusDto> PayRemainingAsync(
            Guid userId,
            FinalizePreOrderPaymentRequest request
        );
        Task<List<PreOrderSummaryDto>> GetMyPreOrdersAsync(Guid userId);
        Task<List<PreOrderSummaryDto>> GetShopPreOrdersAsync(Guid shopUserId);
        Task<int> ExpireOverduePreOrdersAsync();
    }
}
