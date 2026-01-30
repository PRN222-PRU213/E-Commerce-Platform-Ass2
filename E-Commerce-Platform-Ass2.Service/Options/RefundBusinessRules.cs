namespace E_Commerce_Platform_Ass2.Service.Options
{
    /// <summary>
    /// Business rules cho hoàn tiền - có thể cấu hình từ appsettings.json
    /// </summary>
    public class RefundBusinessRules
    {
        public const string SectionName = "RefundBusinessRules";

        #region Time Limits

        /// <summary>
        /// Số ngày tối đa để yêu cầu hoàn tiền sau khi hoàn thành đơn hàng
        /// </summary>
        public int RefundDeadlineDays { get; set; } = 15;

        /// <summary>
        /// Số ngày tối đa để yêu cầu đổi trả sau khi hoàn thành đơn hàng
        /// </summary>
        public int ReturnDeadlineDays { get; set; } = 15;

        #endregion

        #region Fees (percentage)

        /// <summary>
        /// Phí hoàn tiền khi lỗi thuộc về khách hàng (ví dụ: đổi ý)
        /// </summary>
        public decimal CustomerFaultFeePercent { get; set; } = 5;

        /// <summary>
        /// Phí hoàn tiền khi lỗi thuộc về shop (ví dụ: hàng lỗi, giao sai)
        /// </summary>
        public decimal ShopFaultFeePercent { get; set; } = 0;

        #endregion

        #region Limits

        /// <summary>
        /// Số lần tối đa yêu cầu hoàn tiền cho mỗi đơn hàng
        /// </summary>
        public int MaxRefundPerOrder { get; set; } = 1;

        /// <summary>
        /// Số lần tối đa yêu cầu hoàn tiền trong một tháng cho mỗi khách hàng
        /// </summary>
        public int MaxRefundPerMonth { get; set; } = 5;

        #endregion

        #region Reason Mappings

        /// <summary>
        /// Các lý do được xác định là lỗi của Shop (không tính phí khách)
        /// </summary>
        public string[] ShopFaultReasons { get; set; } = { "Defective", "WrongItem", "NotAsDescribed" };

        /// <summary>
        /// Các lý do được xác định là lỗi của khách hàng (tính phí)
        /// </summary>
        public string[] CustomerFaultReasons { get; set; } = { "ChangeOfMind", "Other" };

        #endregion

        #region Helper Methods

        /// <summary>
        /// Kiểm tra lý do có phải lỗi của shop không
        /// </summary>
        public bool IsShopFault(string reason)
        {
            return ShopFaultReasons?.Contains(reason, StringComparer.OrdinalIgnoreCase) ?? false;
        }

        /// <summary>
        /// Tính phí hoàn tiền dựa trên lý do
        /// </summary>
        public decimal CalculateFee(decimal amount, string reason)
        {
            var feePercent = IsShopFault(reason) ? ShopFaultFeePercent : CustomerFaultFeePercent;
            return amount * feePercent / 100;
        }

        /// <summary>
        /// Tính số tiền thực nhận sau khi trừ phí
        /// </summary>
        public decimal CalculateFinalAmount(decimal amount, string reason)
        {
            return amount - CalculateFee(amount, reason);
        }

        #endregion
    }
}
