using E_Commerce_Platform_Ass2.Data.Database.Entities;

namespace E_Commerce_Platform_Ass2.Data.Repositories.Interfaces
{
    public interface IReturnRequestRepository
    {
        Task<ReturnRequest?> GetByIdAsync(Guid id);
        Task<ReturnRequest?> GetByIdWithDetailsAsync(Guid id);
        Task<ReturnRequest?> GetByOrderIdAsync(Guid orderId);
        Task<IEnumerable<ReturnRequest>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<ReturnRequest>> GetByShopIdAsync(Guid shopId, string? status = null);
        Task<bool> ExistsByOrderIdAsync(Guid orderId);
        Task<int> CountByOrderIdAsync(Guid orderId);
        Task<int> CountByUserIdThisMonthAsync(Guid userId);
        Task AddAsync(ReturnRequest request);
        Task UpdateAsync(ReturnRequest request);
    }
}
