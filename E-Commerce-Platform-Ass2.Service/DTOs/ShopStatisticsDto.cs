namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    /// <summary>
    /// DTO thống kê tổng quan của Shop
    /// </summary>
    public class ShopStatisticsDto
    {
        // Thông tin Shop
        public Guid ShopId { get; set; }
        public string ShopName { get; set; } = string.Empty;

        // Thống kê tổng quan
        public int TotalOrders { get; set; }
        public int TotalProductsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal CompletedRevenue { get; set; }

        // Thống kê tuần này
        public int WeeklyOrders { get; set; }
        public int WeeklyProductsSold { get; set; }
        public decimal WeeklyRevenue { get; set; }
        public int WeeklyCompletedOrders { get; set; }
        public int WeeklyCancelledOrders { get; set; }
        public int WeeklyPendingOrders { get; set; }

        // Thống kê tháng này
        public int MonthlyOrders { get; set; }
        public int MonthlyProductsSold { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public int MonthlyCompletedOrders { get; set; }
        public int MonthlyCancelledOrders { get; set; }
        public int MonthlyPendingOrders { get; set; }

        // Thống kê sản phẩm
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int PendingProducts { get; set; }

        // Dữ liệu biểu đồ 7 ngày
        public List<DailySalesDto> DailySales { get; set; } = new();

        // Top sản phẩm bán chạy
        public List<ProductRevenueDto> TopSellingProducts { get; set; } = new();

        // Thống kê doanh thu theo sản phẩm
        public List<ProductRevenueDto> ProductRevenues { get; set; } = new();

        // Đơn hàng gần đây
        public List<RecentOrderDto> RecentOrders { get; set; } = new();
    }

    /// <summary>
    /// DTO doanh thu theo sản phẩm
    /// </summary>
    public class ProductRevenueDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public int TotalQuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
        public int OrderCount { get; set; }

        // Thống kê tuần này
        public int WeeklyQuantitySold { get; set; }
        public decimal WeeklyRevenue { get; set; }

        // Thống kê tháng này
        public int MonthlyQuantitySold { get; set; }
        public decimal MonthlyRevenue { get; set; }
    }
}
