using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Wed.Models;

namespace E_Commerce_Platform_Ass2.Wed.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods để mapping từ Service DTOs sang Web ViewModels cho Shop
    /// </summary>
    public static class ShopMappingExtensions
    {
        #region Shop Dashboard Mapping

        public static ShopDashboardViewModel ToViewModel(this ShopDetailDto dto)
        {
            return new ShopDashboardViewModel
            {
                Id = dto.Id,
                UserId = dto.UserId,
                ShopName = dto.ShopName,
                Description = dto.Description,
                Status = dto.Status,
                CreatedAt = dto.CreatedAt,
                Products = dto.Products?.Select(p => p.ToShopProductViewModel()).ToList() ?? new()
            };
        }

        public static ShopProductViewModel ToShopProductViewModel(this ProductDto dto)
        {
            return new ShopProductViewModel
            {
                Id = dto.Id,
                ShopId = dto.ShopId,
                CategoryId = dto.CategoryId,
                Name = dto.Name,
                Description = dto.Description,
                BasePrice = dto.BasePrice,
                Status = dto.Status,
                AvgRating = dto.AvgRating,
                ImageUrl = dto.ImageUrl,
                CreatedAt = dto.CreatedAt,
                CategoryName = dto.CategoryName,
                ShopName = dto.ShopName
            };
        }

        public static List<ShopProductViewModel> ToShopProductViewModels(this IEnumerable<ProductDto> dtos)
        {
            return dtos?.Select(d => d.ToShopProductViewModel()).ToList() ?? new();
        }

        #endregion

        #region Shop Statistics Mapping

        public static ShopStatisticsViewModel ToViewModel(this ShopStatisticsDto dto)
        {
            return new ShopStatisticsViewModel
            {
                ShopId = dto.ShopId,
                ShopName = dto.ShopName,

                // Thống kê tổng quan
                TotalOrders = dto.TotalOrders,
                TotalProductsSold = dto.TotalProductsSold,
                TotalRevenue = dto.TotalRevenue,
                CompletedRevenue = dto.CompletedRevenue,

                // Thống kê tuần này
                WeeklyOrders = dto.WeeklyOrders,
                WeeklyProductsSold = dto.WeeklyProductsSold,
                WeeklyRevenue = dto.WeeklyRevenue,
                WeeklyCompletedOrders = dto.WeeklyCompletedOrders,
                WeeklyCancelledOrders = dto.WeeklyCancelledOrders,
                WeeklyPendingOrders = dto.WeeklyPendingOrders,

                // Thống kê tháng này
                MonthlyOrders = dto.MonthlyOrders,
                MonthlyProductsSold = dto.MonthlyProductsSold,
                MonthlyRevenue = dto.MonthlyRevenue,
                MonthlyCompletedOrders = dto.MonthlyCompletedOrders,
                MonthlyCancelledOrders = dto.MonthlyCancelledOrders,
                MonthlyPendingOrders = dto.MonthlyPendingOrders,

                // Thống kê sản phẩm
                TotalProducts = dto.TotalProducts,
                ActiveProducts = dto.ActiveProducts,
                PendingProducts = dto.PendingProducts,

                // Collections
                DailySales = dto.DailySales?.Select(d => d.ToShopDailySalesViewModel()).ToList() ?? new(),
                TopSellingProducts = dto.TopSellingProducts?.Select(p => p.ToViewModel()).ToList() ?? new(),
                ProductRevenues = dto.ProductRevenues?.Select(p => p.ToViewModel()).ToList() ?? new(),
                RecentOrders = dto.RecentOrders?.Select(o => o.ToShopRecentOrderViewModel()).ToList() ?? new()
            };
        }

        public static DailySalesViewModel ToShopDailySalesViewModel(this DailySalesDto dto)
        {
            return new DailySalesViewModel
            {
                Date = dto.Date,
                DateLabel = dto.DateLabel,
                OrderCount = dto.OrderCount,
                Revenue = dto.Revenue
            };
        }

        public static ProductRevenueViewModel ToViewModel(this ProductRevenueDto dto)
        {
            return new ProductRevenueViewModel
            {
                ProductId = dto.ProductId,
                ProductName = dto.ProductName,
                ImageUrl = dto.ImageUrl,
                TotalQuantitySold = dto.TotalQuantitySold,
                TotalRevenue = dto.TotalRevenue,
                OrderCount = dto.OrderCount,
                WeeklyQuantitySold = dto.WeeklyQuantitySold,
                WeeklyRevenue = dto.WeeklyRevenue,
                MonthlyQuantitySold = dto.MonthlyQuantitySold,
                MonthlyRevenue = dto.MonthlyRevenue
            };
        }

        public static ShopRecentOrderViewModel ToShopRecentOrderViewModel(this RecentOrderDto dto)
        {
            return new ShopRecentOrderViewModel
            {
                Id = dto.Id,
                OrderCode = dto.OrderCode,
                CustomerName = dto.CustomerName,
                ShopName = dto.ShopName,
                TotalAmount = dto.TotalAmount,
                Status = dto.Status,
                CreatedAt = dto.CreatedAt
            };
        }

        #endregion
    }
}
