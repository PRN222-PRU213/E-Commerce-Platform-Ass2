using E_Commerce_Platform_Ass2.Data.Database.Entities;

namespace E_Commerce_Platform_Ass2.Data.Repositories.Interfaces
{
    public interface IPreOrderPolicyItemRepository
    {
        Task<PreOrderPolicyItem?> GetByVariantIdAsync(Guid productVariantId);
        Task<List<PreOrderPolicyItem>> GetActiveByProductIdAsync(Guid productId);
        Task<PreOrderPolicyItem> AddAsync(PreOrderPolicyItem item);
        Task<PreOrderPolicyItem> UpdateAsync(PreOrderPolicyItem item);
    }
}
