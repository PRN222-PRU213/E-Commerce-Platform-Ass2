using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Models;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    /// <summary>
    /// Service interface cho quản lý biến thể sản phẩm
    /// </summary>
    public interface IProductVariantService
    {
        /// <summary>
        /// Lấy danh sách biến thể theo ProductId
        /// </summary>
        Task<ServiceResult<List<ProductVariantDto>>> GetByProductIdAsync(Guid productId);

        /// <summary>
        /// Lấy biến thể theo Id
        /// </summary>
        Task<ServiceResult<ProductVariantDto>> GetByIdAsync(Guid variantId);

        /// <summary>
        /// Thêm biến thể cho sản phẩm
        /// </summary>
        Task<ServiceResult<Guid>> AddVariantAsync(CreateProductVariantDto dto, Guid shopId);

        /// <summary>
        /// Cập nhật biến thể
        /// </summary>
        Task<ServiceResult> UpdateVariantAsync(UpdateProductVariantDto dto, Guid shopId);

        /// <summary>
        /// Xóa biến thể
        /// </summary>
        Task<ServiceResult> DeleteVariantAsync(Guid variantId, Guid shopId);

        /// <summary>
        /// Kiểm tra sản phẩm có biến thể hay không
        /// </summary>
        Task<bool> HasVariantsAsync(Guid productId);
    }
}
