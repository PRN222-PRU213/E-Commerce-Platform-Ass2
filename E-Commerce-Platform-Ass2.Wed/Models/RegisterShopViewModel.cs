using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Platform_Ass2.Wed.Models
{
    public class RegisterShopViewModel
    {
        [Required(ErrorMessage = "Tên shop là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên shop phải từ {2} đến {1} ký tự.", MinimumLength = 2)]
        [Display(Name = "Tên shop")]
        public string ShopName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mô tả shop là bắt buộc.")]
        [StringLength(500, ErrorMessage = "Mô tả không được vượt quá {1} ký tự.", MinimumLength = 10)]
        [Display(Name = "Mô tả shop")]
        public string Description { get; set; } = string.Empty;
    }
}
