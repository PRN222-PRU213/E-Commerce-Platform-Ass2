using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Models;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    /// <summary>
    /// Service interface cho quản lý sản phẩm
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Lấy tất cả sản phẩm (dùng cho Home page) - trả về DTO
        /// </summary>
        Task<List<ProductDto>> GetAllProductsAsync();

        /// <summary>
        /// Lấy tất cả sản phẩm (legacy - trả về Entity) - sẽ deprecated
        /// </summary>
        [Obsolete("Use GetAllProductsAsync() instead")]
        Task<List<Product>> GetAllProductAsync();

        Task<Product?> GetProductWithVariantsAsync(Guid productId);

        /// <summary>
        /// Lấy chi tiết sản phẩm bao gồm variants (dùng cho Product Detail page) - trả về DTO
        /// </summary>
        Task<ProductDetailDto?> GetProductDetailDtoAsync(Guid productId);

        Task<ServiceResult<Guid>> CreateProductAsync(CreateProductDto dto);

        Task<ServiceResult<List<ProductDto>>> GetByShopIdAsync(Guid shopId);

        /// <summary>
        /// Lấy sản phẩm theo Id
        /// </summary>
        Task<ServiceResult<ProductDto>> GetByIdAsync(Guid productId);

        /// <summary>
        /// Lấy chi tiết sản phẩm bao gồm variants (dùng cho trang Edit)
        /// </summary>
        Task<ServiceResult<ProductDetailDto>> GetProductDetailAsync(Guid productId, Guid shopId);

        /// <summary>
        /// Submit sản phẩm để admin duyệt (chuyển từ draft sang pending)
        /// </summary>
        Task<ServiceResult> SubmitProductAsync(Guid productId, Guid shopId);

        /// <summary>
        /// Cập nhật thông tin sản phẩm (chỉ khi ở trạng thái draft)
        /// </summary>
        Task<ServiceResult> UpdateProductAsync(UpdateProductDto dto);

        /// <summary>
        /// Gỡ sản phẩm (chuyển từ active về draft để chỉnh sửa)
        /// </summary>
        Task<ServiceResult> UnpublishProductAsync(Guid productId, Guid shopId);

        /// <summary>
        /// Lấy tất cả danh mục active
        /// </summary>
        Task<List<CategoryDto>> GetAllCategoriesAsync();
    }
}
