using System.Collections.Generic;
using System.Linq;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Wed.Models;

namespace E_Commerce_Platform_Ass2.Wed.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods để mapping từ Service DTOs sang Web ViewModels
    /// </summary>
    public static class AdminMappingExtensions
    {
        #region Dashboard Mapping

        public static AdminDashboardViewModel ToViewModel(this AdminDashboardDto dto)
        {
            return new AdminDashboardViewModel
            {
                // Shops
                TotalShops = dto.TotalShops,
                ActiveShops = dto.ActiveShops,
                PendingShops = dto.PendingShops,
                InactiveShops = dto.InactiveShops,

                // Products
                TotalProducts = dto.TotalProducts,
                ActiveProducts = dto.ActiveProducts,
                PendingProducts = dto.PendingProducts,
                DraftProducts = dto.DraftProducts,
                RejectedProducts = dto.RejectedProducts,

                // Users
                TotalUsers = dto.TotalUsers,

                // Orders
                TotalOrders = dto.TotalOrders,
                TotalRevenue = dto.TotalRevenue,

                // Weekly
                WeeklyOrders = dto.WeeklyOrders,
                WeeklyRevenue = dto.WeeklyRevenue,
                WeeklyCompletedOrders = dto.WeeklyCompletedOrders,
                WeeklyCancelledOrders = dto.WeeklyCancelledOrders,

                // Monthly
                MonthlyOrders = dto.MonthlyOrders,
                MonthlyRevenue = dto.MonthlyRevenue,
                MonthlyCompletedOrders = dto.MonthlyCompletedOrders,
                MonthlyCancelledOrders = dto.MonthlyCancelledOrders,

                // Statistics
                TopShopsByRevenue = dto.TopShopsByRevenue?.Select(x => x.ToViewModel()).ToList() ?? new(),
                ShopWeeklyStats = dto.ShopWeeklyStats?.Select(x => x.ToViewModel()).ToList() ?? new(),
                ShopMonthlyStats = dto.ShopMonthlyStats?.Select(x => x.ToViewModel()).ToList() ?? new(),
                DailySalesStats = dto.DailySalesStats?.Select(x => x.ToViewModel()).ToList() ?? new(),

                // Recent items
                RecentShops = dto.RecentShops?.Select(x => x.ToViewModel()).ToList() ?? new(),
                RecentPendingProducts = dto.RecentPendingProducts?.Select(x => x.ToViewModel()).ToList() ?? new(),
                RecentOrders = dto.RecentOrders?.Select(x => x.ToViewModel()).ToList() ?? new()
            };
        }

        public static ShopSalesStatViewModel ToViewModel(this ShopSalesStatDto dto)
        {
            return new ShopSalesStatViewModel
            {
                ShopId = dto.ShopId,
                ShopName = dto.ShopName,
                OwnerName = dto.OwnerName,
                TotalOrders = dto.TotalOrders,
                CompletedOrders = dto.CompletedOrders,
                CancelledOrders = dto.CancelledOrders,
                PendingOrders = dto.PendingOrders,
                TotalRevenue = dto.TotalRevenue,
                CompletedRevenue = dto.CompletedRevenue,
                TotalProductsSold = dto.TotalProductsSold
            };
        }

        public static DailySalesViewModel ToViewModel(this DailySalesDto dto)
        {
            return new DailySalesViewModel
            {
                Date = dto.Date,
                DateLabel = dto.DateLabel,
                OrderCount = dto.OrderCount,
                Revenue = dto.Revenue
            };
        }

        public static RecentOrderViewModel ToViewModel(this RecentOrderDto dto)
        {
            return new RecentOrderViewModel
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

        #region Shop Mapping

        public static AdminShopViewModel ToViewModel(this ShopDto dto)
        {
            return new AdminShopViewModel
            {
                Id = dto.Id,
                UserId = dto.UserId,
                ShopName = dto.ShopName,
                Description = dto.Description,
                Status = dto.Status,
                CreatedAt = dto.CreatedAt,
                OwnerName = dto.OwnerName,
                OwnerEmail = dto.OwnerEmail,
                ProductCount = dto.ProductCount
            };
        }

        public static List<AdminShopViewModel> ToViewModels(this IEnumerable<ShopDto> dtos)
        {
            return dtos?.Select(x => x.ToViewModel()).ToList() ?? new();
        }

        public static AdminShopDetailViewModel ToDetailViewModel(this ShopDetailDto dto)
        {
            return new AdminShopDetailViewModel
            {
                Id = dto.Id,
                UserId = dto.UserId,
                ShopName = dto.ShopName,
                Description = dto.Description,
                Status = dto.Status,
                CreatedAt = dto.CreatedAt,
                OwnerName = dto.OwnerName,
                OwnerEmail = dto.OwnerEmail,
                ProductCount = dto.ProductCount,
                Products = dto.Products?.Select(x => x.ToViewModel()).ToList() ?? new()
            };
        }

        #endregion

        #region Product Mapping

        public static AdminProductViewModel ToViewModel(this ProductDto dto)
        {
            return new AdminProductViewModel
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

        public static List<AdminProductViewModel> ToViewModels(this IEnumerable<ProductDto> dtos)
        {
            return dtos?.Select(x => x.ToViewModel()).ToList() ?? new();
        }

        public static AdminProductDetailViewModel ToDetailViewModel(this ProductDetailDto dto)
        {
            return new AdminProductDetailViewModel
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
                ShopName = dto.ShopName,
                Variants = dto.Variants?.Select(x => x.ToViewModel()).ToList() ?? new()
            };
        }

        public static AdminProductVariantViewModel ToViewModel(this ProductVariantDto dto)
        {
            return new AdminProductVariantViewModel
            {
                Id = dto.Id,
                ProductId = dto.ProductId,
                VariantName = dto.VariantName,
                Price = dto.Price,
                Size = dto.Size,
                Color = dto.Color,
                Stock = dto.Stock,
                Sku = dto.Sku,
                Status = dto.Status,
                ImageUrl = dto.ImageUrl
            };
        }

        #endregion

        #region Category Mapping

        public static AdminCategoryViewModel ToViewModel(this CategoryDto dto)
        {
            return new AdminCategoryViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Status = dto.Status,
                ProductCount = dto.ProductCount
            };
        }

        public static List<AdminCategoryViewModel> ToViewModels(this IEnumerable<CategoryDto> dtos)
        {
            return dtos?.Select(x => x.ToViewModel()).ToList() ?? new();
        }

        public static EditCategoryViewModel ToEditViewModel(this CategoryDto dto)
        {
            return new EditCategoryViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Status = dto.Status
            };
        }

        public static CreateCategoryDto ToDto(this CreateCategoryViewModel vm)
        {
            return new CreateCategoryDto
            {
                Name = vm.Name,
                Status = vm.Status
            };
        }

        public static UpdateCategoryDto ToDto(this EditCategoryViewModel vm)
        {
            return new UpdateCategoryDto
            {
                Name = vm.Name,
                Status = vm.Status
            };
        }

        #endregion
    }
}
