namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface IMomoService
    {
        Task<string> CreatePaymentAsync(long amount, string orderInfo);
        
        /// <summary>
        /// Tạo payment URL cho nạp tiền ví với callback URL riêng
        /// </summary>
        Task<string> CreateTopUpPaymentAsync(long amount, string orderInfo);
    }
}
