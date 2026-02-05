using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Platform_Ass2.Wed.Models
{
    public class TopUpViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập số tiền nạp.")]
        [Range(10000, 10000000, ErrorMessage = "Số tiền nạp từ 10,000 đến 10,000,000 VNĐ.")]
        [Display(Name = "Số tiền nạp")]
        public decimal Amount { get; set; }
    }
}
