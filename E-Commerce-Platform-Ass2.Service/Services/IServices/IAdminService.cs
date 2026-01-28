using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Models;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    /// <summary>
    /// Service interface cho Admin quản lý
    /// </summary>
    public interface IAdminService
    {
        #region Shop Management

        /// <summary>
        /// Lấy tất cả shops
        /// </summary>
        Task<ServiceResult<List<ShopDto>>> GetAllShopsAsync();

        /// <summary>
        /// Lấy shops theo status
        /// </summary>
        Task<ServiceResult<List<ShopDto>>> GetShopsByStatusAsync(string status);

        /// <summary>
        /// Lấy chi tiết shop
        /// </summary>
        Task<ServiceResult<ShopDetailDto>> GetShopDetailAsync(Guid shopId);

        /// <summary>
        /// Duyệt shop (Active)
        /// </summary>
        Task<ServiceResult> ApproveShopAsync(Guid shopId);

        /// <summary>
        /// Từ chối/Khóa shop (Inactive)
        /// </summary>
        Task<ServiceResult> RejectShopAsync(Guid shopId, string? reason = null);

        #endregion

        #region Product Approval

        /// <summary>
        /// Lấy danh sách sản phẩm chờ duyệt (status = pending)
        /// </summary>
        Task<ServiceResult<List<ProductDto>>> GetPendingProductsAsync();

        /// <summary>
        /// Lấy tất cả sản phẩm (cho admin xem)
        /// </summary>
        Task<ServiceResult<List<ProductDto>>> GetAllProductsAsync();

        /// <summary>
        /// Lấy sản phẩm theo status
        /// </summary>
        Task<ServiceResult<List<ProductDto>>> GetProductsByStatusAsync(string status);

        /// <summary>
        /// Lấy chi tiết sản phẩm để duyệt
        /// </summary>
        Task<ServiceResult<ProductDetailDto>> GetProductForApprovalAsync(Guid productId);

        /// <summary>
        /// Duyệt sản phẩm (chuyển từ pending sang active)
        /// </summary>
        Task<ServiceResult> ApproveProductAsync(Guid productId);

        /// <summary>
        /// Từ chối sản phẩm (chuyển từ pending sang rejected)
        /// </summary>
        Task<ServiceResult> RejectProductAsync(Guid productId, string? reason = null);

        #endregion

        #region Category Management

        /// <summary>
        /// Lấy tất cả categories
        /// </summary>
        Task<ServiceResult<List<CategoryDto>>> GetAllCategoriesAsync();

        /// <summary>
        /// Lấy category theo Id
        /// </summary>
        Task<ServiceResult<CategoryDto>> GetCategoryByIdAsync(Guid categoryId);

        /// <summary>
        /// Tạo category mới
        /// </summary>
        Task<ServiceResult<Guid>> CreateCategoryAsync(CreateCategoryDto dto);

        /// <summary>
        /// Cập nhật category
        /// </summary>
        Task<ServiceResult> UpdateCategoryAsync(Guid categoryId, UpdateCategoryDto dto);

        /// <summary>
        /// Xóa category
        /// </summary>
        Task<ServiceResult> DeleteCategoryAsync(Guid categoryId);

        #endregion

        #region Statistics

        /// <summary>
        /// Lấy thống kê tổng quan cho dashboard
        /// </summary>
        Task<AdminDashboardDto> GetDashboardStatisticsAsync();

        #endregion
    }
}
