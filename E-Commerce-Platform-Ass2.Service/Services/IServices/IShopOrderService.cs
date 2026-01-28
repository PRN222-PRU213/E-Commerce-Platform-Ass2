using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Models;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    /// <summary>
    /// Service xử lý đơn hàng cho Shop Owner
    /// </summary>
    public interface IShopOrderService
    {
        /// <summary>
        /// Lấy danh sách đơn hàng của shop
        /// </summary>
        Task<ServiceResult<List<OrderDto>>> GetOrdersByShopIdAsync(Guid shopId);

        /// <summary>
        /// Lấy chi tiết đơn hàng
        /// </summary>
        Task<ServiceResult<OrderDetailDto>> GetOrderDetailAsync(Guid orderId, Guid shopId);

        /// <summary>
        /// Bắt đầu xử lý đơn hàng (Pending → Processing)
        /// </summary>
        Task<ServiceResult> StartProcessingAsync(Guid orderId, Guid shopId);

        /// <summary>
        /// Bắt đầu chuẩn bị hàng (Processing → Preparing)
        /// </summary>
        Task<ServiceResult> StartPreparingAsync(Guid orderId, Guid shopId);

        /// <summary>
        /// Xác nhận đơn hàng (Legacy - để tương thích)
        /// </summary>
        Task<ServiceResult> ConfirmOrderAsync(Guid orderId, Guid shopId);

        /// <summary>
        /// Gửi hàng - Tạo shipment và chuyển trạng thái sang Shipped
        /// </summary>
        Task<ServiceResult> ShipOrderAsync(Guid orderId, Guid shopId, CreateShipmentDto dto);

        /// <summary>
        /// Cập nhật trạng thái vận chuyển
        /// </summary>
        Task<ServiceResult> UpdateShipmentAsync(Guid orderId, Guid shopId, UpdateShipmentDto dto);

        /// <summary>
        /// Đánh dấu đã giao hàng
        /// </summary>
        Task<ServiceResult> MarkAsDeliveredAsync(Guid orderId, Guid shopId);

        /// <summary>
        /// Từ chối đơn hàng
        /// </summary>
        Task<ServiceResult> RejectOrderAsync(Guid orderId, Guid shopId, string? reason);

        /// <summary>
        /// Lấy thống kê đơn hàng của shop
        /// </summary>
        Task<ShopOrderStatistics> GetOrderStatisticsAsync(Guid shopId);
    }

    /// <summary>
    /// Thống kê đơn hàng shop
    /// </summary>
    public class ShopOrderStatistics
    {
        public int TotalOrders { get; set; }
        public int PendingOrders { get; set; }
        public int ConfirmedOrders { get; set; }
        public int ShippingOrders { get; set; }
        public int DeliveredOrders { get; set; }
        public int CancelledOrders { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
