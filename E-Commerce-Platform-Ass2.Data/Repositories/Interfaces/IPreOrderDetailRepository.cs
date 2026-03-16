using E_Commerce_Platform_Ass2.Data.Database.Entities;

namespace E_Commerce_Platform_Ass2.Data.Repositories.Interfaces
{
    public interface IPreOrderDetailRepository
    {
        Task<PreOrderDetail?> GetByIdAsync(Guid id);
        Task<PreOrderDetail?> GetByIdWithOrderAsync(Guid id);
        Task<PreOrderDetail> AddAsync(PreOrderDetail detail);
        Task<PreOrderDetail> UpdateAsync(PreOrderDetail detail);
        Task<IEnumerable<PreOrderDetail>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<PreOrderDetail>> GetByShopIdAsync(Guid shopId);
        Task<int> GetReservedQuantityByVariantAsync(Guid productVariantId);
    }
}
