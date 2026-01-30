using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Models;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface IShopWalletService
    {
        /// <summary>
        /// Lấy hoặc tạo ví Shop
        /// </summary>
        Task<ShopWalletDto> GetOrCreateAsync(Guid shopId);

        /// <summary>
        /// Nhận tiền từ đơn hàng
        /// </summary>
        Task<ServiceResult> ReceiveOrderPaymentAsync(Guid shopId, Guid orderId, decimal amount);

        /// <summary>
        /// Hoàn tiền đơn hàng (khi khách return)
        /// </summary>
        Task<ServiceResult> RefundOrderPaymentAsync(Guid shopId, Guid orderId, decimal amount);

        /// <summary>
        /// Lấy lịch sử giao dịch
        /// </summary>
        Task<List<ShopWalletTransactionDto>> GetTransactionsAsync(Guid shopId, int limit = 20);

        /// <summary>
        /// Phân phối tiền đơn hàng cho các shops
        /// </summary>
        Task DistributeOrderPaymentAsync(Guid orderId);
    }
}
