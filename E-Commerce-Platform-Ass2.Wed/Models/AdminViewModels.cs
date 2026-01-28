using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce_Platform_Ass2.Wed.Models
{
    #region Dashboard ViewModels

    public class AdminDashboardViewModel
    {
        public int TotalShops { get; set; }
        public int ActiveShops { get; set; }
        public int PendingShops { get; set; }
        public int InactiveShops { get; set; }

        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int PendingProducts { get; set; }
        public int DraftProducts { get; set; }
        public int RejectedProducts { get; set; }

        public int TotalUsers { get; set; }

        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        public int WeeklyOrders { get; set; }
        public decimal WeeklyRevenue { get; set; }
        public int WeeklyCompletedOrders { get; set; }
        public int WeeklyCancelledOrders { get; set; }

        public int MonthlyOrders { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int MonthlyCompletedOrders { get; set; }
        public int MonthlyCancelledOrders { get; set; }

        public List<ShopSalesStatViewModel> TopShopsByRevenue { get; set; } = new();
        public List<ShopSalesStatViewModel> ShopWeeklyStats { get; set; } = new();
        public List<ShopSalesStatViewModel> ShopMonthlyStats { get; set; } = new();

        public List<DailySalesViewModel> DailySalesStats { get; set; } = new();

        public List<AdminShopViewModel> RecentShops { get; set; } = new();
        public List<AdminProductViewModel> RecentPendingProducts { get; set; } = new();
        public List<RecentOrderViewModel> RecentOrders { get; set; } = new();
    }

    public class ShopSalesStatViewModel
    {
        public Guid ShopId { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public string? OwnerName { get; set; }
        public int TotalOrders { get; set; }
        public int CompletedOrders { get; set; }
        public int CancelledOrders { get; set; }
        public int PendingOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal CompletedRevenue { get; set; }
        public int TotalProductsSold { get; set; }
    }

    public class RecentOrderViewModel
    {
        public Guid Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public string? ShopName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    #endregion

    #region Shop ViewModels

    public class AdminShopViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? OwnerName { get; set; }
        public string? OwnerEmail { get; set; }
        public int ProductCount { get; set; }
    }

    public class AdminShopDetailViewModel : AdminShopViewModel
    {
        public List<AdminProductViewModel> Products { get; set; } = new();
    }

    #endregion

    #region Product ViewModels

    public class AdminProductViewModel
    {
        public Guid Id { get; set; }
        public Guid ShopId { get; set; }
        public Guid CategoryId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal AvgRating { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? CategoryName { get; set; }
        public string? ShopName { get; set; }
    }

    public class AdminProductDetailViewModel : AdminProductViewModel
    {
        public List<AdminProductVariantViewModel> Variants { get; set; } = new();
    }

    public class AdminProductVariantViewModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string VariantName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public int Stock { get; set; }
        public string Sku { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }

    #endregion

    #region Category ViewModels

    public class AdminCategoryViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int ProductCount { get; set; }
    }

    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên danh mục tối đa 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        public string Status { get; set; } = "Active";
    }

    public class EditCategoryViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên danh mục tối đa 100 ký tự")]
        public string Name { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }

    #endregion
}

