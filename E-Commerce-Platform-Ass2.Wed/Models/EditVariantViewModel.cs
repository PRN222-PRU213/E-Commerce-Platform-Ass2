using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Platform_Ass2.Wed.Models
{
    public class EditVariantViewModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên biến thể là bắt buộc.")]
        [StringLength(200, ErrorMessage = "Tên biến thể không được vượt quá {1} ký tự.")]
        [Display(Name = "Tên biến thể")]
        public string VariantName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Giá là bắt buộc.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0.")]
        [Display(Name = "Giá (₫)")]
        public decimal Price { get; set; }

        [StringLength(50, ErrorMessage = "Kích thước không được vượt quá {1} ký tự.")]
        [Display(Name = "Kích thước")]
        public string? Size { get; set; }

        [StringLength(50, ErrorMessage = "Màu sắc không được vượt quá {1} ký tự.")]
        [Display(Name = "Màu sắc")]
        public string? Color { get; set; }

        [Required(ErrorMessage = "Số lượng tồn kho là bắt buộc.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn hoặc bằng 0.")]
        [Display(Name = "Tồn kho")]
        public int Stock { get; set; }

        [StringLength(100, ErrorMessage = "SKU không được vượt quá {1} ký tự.")]
        [Display(Name = "SKU")]
        public string? Sku { get; set; }

        [StringLength(500, ErrorMessage = "URL hình ảnh không được vượt quá {1} ký tự.")]
        [Display(Name = "Link hình ảnh")]
        [Url(ErrorMessage = "URL không hợp lệ.")]
        public string? ImageUrl { get; set; }
    }
}
