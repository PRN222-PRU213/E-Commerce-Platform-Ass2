namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    /// <summary>
    /// DTO cho Dashboard Admin
    /// </summary>
    public class AdminDashboardDto
    {
        // Shops
        public int TotalShops { get; set; }
        public int ActiveShops { get; set; }
        public int PendingShops { get; set; }
        public int InactiveShops { get; set; }

        // Products
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int PendingProducts { get; set; }
        public int DraftProducts { get; set; }
        public int RejectedProducts { get; set; }

        // Users
        public int TotalUsers { get; set; }

        // Orders Statistics
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }

        // This Week Statistics
        public int WeeklyOrders { get; set; }
        public decimal WeeklyRevenue { get; set; }
        public int WeeklyCompletedOrders { get; set; }
        public int WeeklyCancelledOrders { get; set; }

        // This Month Statistics
        public int MonthlyOrders { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int MonthlyCompletedOrders { get; set; }
        public int MonthlyCancelledOrders { get; set; }

        // Shop Sales Statistics
        public List<ShopSalesStatDto> TopShopsByRevenue { get; set; } = new();
        public List<ShopSalesStatDto> ShopWeeklyStats { get; set; } = new();
        public List<ShopSalesStatDto> ShopMonthlyStats { get; set; } = new();

        // Daily Statistics for Chart (last 7 days)
        public List<DailySalesDto> DailySalesStats { get; set; } = new();

        // Recent items
        public List<ShopDto> RecentShops { get; set; } = new();
        public List<ProductDto> RecentPendingProducts { get; set; } = new();
        public List<RecentOrderDto> RecentOrders { get; set; } = new();
    }

    /// <summary>
    /// DTO thống kê bán hàng theo shop
    /// </summary>
    public class ShopSalesStatDto
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

    /// <summary>
    /// DTO thống kê doanh số theo ngày
    /// </summary>
    public class DailySalesDto
    {
        public DateTime Date { get; set; }
        public string DateLabel { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }

    /// <summary>
    /// DTO đơn hàng gần đây
    /// </summary>
    public class RecentOrderDto
    {
        public Guid Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public string? ShopName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
