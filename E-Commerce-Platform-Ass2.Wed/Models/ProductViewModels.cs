using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Commerce_Platform_Ass2.Wed.Models
{
    /// <summary>
    /// ViewModel cho form tạo sản phẩm
    /// </summary>
    public class CreateProductViewModel
    {
        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá {1} ký tự.")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Mô tả không được vượt quá {1} ký tự.")]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0.")]
        [Display(Name = "Giá bán (₫)")]
        public decimal BasePrice { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        [Display(Name = "Danh mục")]
        public Guid CategoryId { get; set; }

        [StringLength(500, ErrorMessage = "URL hình ảnh không được vượt quá {1} ký tự.")]
        [Display(Name = "Link hình ảnh")]
        [Url(ErrorMessage = "URL không hợp lệ.")]
        public string? ImageUrl { get; set; }

        // Dropdown list categories
        public List<SelectListItem> Categories { get; set; } = new();
    }

    /// <summary>
    /// ViewModel hiển thị sản phẩm trong danh sách
    /// </summary>
    public class ProductListItemViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? CategoryName { get; set; }

        // Helper để hiển thị status
        public string StatusBadgeClass =>
            Status switch
            {
                "active" => "badge-success",
                "inactive" => "badge-secondary",
                "draft" => "badge-warning",
                _ => "badge-info",
            };

        public string StatusDisplayName =>
            Status switch
            {
                "active" => "Đang bán",
                "inactive" => "Ngừng bán",
                "draft" => "Bản nháp",
                _ => Status,
            };
    }

    /// <summary>
    /// ViewModel cho trang danh sách sản phẩm
    /// </summary>
    public class ProductListViewModel
    {
        public Guid ShopId { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public List<ProductListItemViewModel> Products { get; set; } = new();

        // Statistics
        public int TotalProducts => Products.Count;
        public int ActiveCount => Products.Count(p => p.Status == "active");
        public int InactiveCount => Products.Count(p => p.Status == "inactive");
        public int DraftCount => Products.Count(p => p.Status == "draft");
        public int PendingCount => Products.Count(p => p.Status == "pending");
    }

    /// <summary>
    /// ViewModel cho biến thể sản phẩm trong trang Edit
    /// </summary>
    public class ProductVariantViewModel
    {
        public Guid Id { get; set; }
        public string VariantName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public int Stock { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }

    /// <summary>
    /// ViewModel cho trang Edit sản phẩm (bao gồm variants)
    /// </summary>
    public class EditProductViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [StringLength(200, ErrorMessage = "Tên sản phẩm không được vượt quá {1} ký tự.")]
        [Display(Name = "Tên sản phẩm")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Mô tả không được vượt quá {1} ký tự.")]
        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc.")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn hoặc bằng 0.")]
        [Display(Name = "Giá bán (₫)")]
        public decimal BasePrice { get; set; }

        public string Status { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "URL hình ảnh không được vượt quá {1} ký tự.")]
        [Display(Name = "Link hình ảnh")]
        [Url(ErrorMessage = "URL không hợp lệ.")]
        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục.")]
        [Display(Name = "Danh mục")]
        public Guid CategoryId { get; set; }

        public string? CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }

        // Dropdown list categories
        public List<SelectListItem> Categories { get; set; } = new();

        public List<ProductVariantViewModel> Variants { get; set; } = new();

        // Helper properties
        public bool CanSubmit => Status == "draft" && Variants.Any();
        public bool CanAddVariant => Status == "draft";

        public string StatusBadgeClass =>
            Status switch
            {
                "active" => "badge-success",
                "inactive" => "badge-secondary",
                "draft" => "badge-warning",
                "pending" => "badge-info",
                _ => "badge-secondary",
            };

        public string StatusDisplayName =>
            Status switch
            {
                "active" => "Đang bán",
                "inactive" => "Ngừng bán",
                "draft" => "Bản nháp",
                "pending" => "Chờ duyệt",
                _ => Status,
            };
    }

    /// <summary>
    /// ViewModel cho form thêm biến thể sản phẩm
    /// </summary>
    public class AddVariantViewModel
    {
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

    /// <summary>
    /// ViewModel cho trang xem chi tiết sản phẩm (readonly)
    /// </summary>
    public class ProductDetailViewModel
    {
        public Guid Id { get; set; }
        public Guid ShopId { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal BasePrice { get; set; }
        public string? ImageUrl { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? CategoryName { get; set; }
        public string? ShopName { get; set; }
        public decimal AvgRating { get; set; }
        public DateTime CreatedAt { get; set; }

        // Variants
        public List<ProductVariantItemViewModel> Variants { get; set; } = new();

        // Reviews
        public List<ReviewViewModel> Reviews { get; set; } = new();

        public int ReviewCount => Reviews.Count;

        // Helper properties
        public string StatusBadgeClass =>
            Status switch
            {
                "active" => "badge-success",
                "inactive" => "badge-secondary",
                "draft" => "badge-warning",
                "pending" => "badge-info",
                _ => "badge-secondary",
            };

        public string StatusDisplayName =>
            Status switch
            {
                "active" => "Đang bán",
                "inactive" => "Ngừng bán",
                "draft" => "Bản nháp",
                "pending" => "Chờ duyệt",
                _ => Status,
            };

        public int TotalStock => Variants.Sum(v => v.Stock);
        public decimal MinPrice => Variants.Any() ? Variants.Min(v => v.Price) : BasePrice;
        public decimal MaxPrice => Variants.Any() ? Variants.Max(v => v.Price) : BasePrice;
    }

    /// <summary>
    /// ViewModel cho Product Variant trong Detail page
    /// </summary>
    public class ProductVariantItemViewModel
    {
        public Guid Id { get; set; }
        public string VariantName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public int Stock { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }

        public bool IsInStock => Stock > 0 && Status == "active";
    }

}
