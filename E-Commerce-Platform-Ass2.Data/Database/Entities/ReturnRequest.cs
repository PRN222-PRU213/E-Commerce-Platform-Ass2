namespace E_Commerce_Platform_Ass2.Data.Database.Entities
{
    /// <summary>
    /// Yêu cầu đổi trả/hoàn tiền từ khách hàng
    /// </summary>
    public class ReturnRequest
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }

        /// <summary>
        /// Loại yêu cầu: Return (trả hàng + hoàn tiền), Refund (chỉ hoàn tiền)
        /// </summary>
        public string RequestType { get; set; } = "Refund";

        /// <summary>
        /// Lý do: Defective, WrongItem, NotAsDescribed, ChangeOfMind, Other
        /// </summary>
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Chi tiết lý do
        /// </summary>
        public string? ReasonDetail { get; set; }

        /// <summary>
        /// URLs hình ảnh bằng chứng (JSON array)
        /// </summary>
        public string? EvidenceImages { get; set; }

        /// <summary>
        /// Số tiền yêu cầu hoàn
        /// </summary>
        public decimal RequestedAmount { get; set; }

        /// <summary>
        /// Số tiền được duyệt (có thể khác RequestedAmount nếu partial refund)
        /// </summary>
        public decimal? ApprovedAmount { get; set; }

        /// <summary>
        /// Trạng thái: Pending, Approved, Rejected, Completed
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Phản hồi từ Shop
        /// </summary>
        public string? ShopResponse { get; set; }

        /// <summary>
        /// Shop xử lý yêu cầu
        /// </summary>
        public Guid? ProcessedByShopId { get; set; }

        public DateTime? ProcessedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public Order Order { get; set; } = null!;
        public User User { get; set; } = null!;
        public Shop? ProcessedByShop { get; set; }
    }
}
