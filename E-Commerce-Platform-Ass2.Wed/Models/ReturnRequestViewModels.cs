using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Platform_Ass2.Wed.Models
{
    /// <summary>
    /// ViewModel để tạo yêu cầu đổi trả
    /// </summary>
    public class CreateReturnRequestViewModel
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItemViewModel> Items { get; set; } = new();

        [Required(ErrorMessage = "Vui lòng chọn loại yêu cầu")]
        [Display(Name = "Loại yêu cầu")]
        public string RequestType { get; set; } = "Refund";

        [Required(ErrorMessage = "Vui lòng chọn lý do")]
        [Display(Name = "Lý do")]
        public string Reason { get; set; } = string.Empty;

        [Display(Name = "Chi tiết lý do")]
        [MaxLength(1000)]
        public string? ReasonDetail { get; set; }

        /// <summary>
        /// Số tiền hoàn (tự động lấy từ TotalAmount)
        /// </summary>
        public decimal RequestedAmount { get; set; }

        // Danh sách lý do phổ biến
        public static List<SelectOption> ReasonOptions => new()
        {
            new SelectOption("Defective", "Sản phẩm bị lỗi/hỏng"),
            new SelectOption("WrongItem", "Giao sai sản phẩm"),
            new SelectOption("NotAsDescribed", "Không đúng mô tả"),
            new SelectOption("ChangeOfMind", "Đổi ý không muốn mua nữa"),
            new SelectOption("Other", "Lý do khác")
        };

        public static List<SelectOption> RequestTypeOptions => new()
        {
            new SelectOption("Refund", "Chỉ hoàn tiền"),
            new SelectOption("Return", "Trả hàng và hoàn tiền")
        };
    }

    public class SelectOption
    {
        public string Value { get; set; }
        public string Text { get; set; }

        public SelectOption(string value, string text)
        {
            Value = value;
            Text = text;
        }
    }
}
