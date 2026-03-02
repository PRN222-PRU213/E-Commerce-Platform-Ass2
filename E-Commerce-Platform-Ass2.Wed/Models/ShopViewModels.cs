namespace E_Commerce_Platform_Ass2.Wed.Models
{
    /// <summary>
    /// ViewModel cho trang ViewShop (Dashboard của shop owner)
    /// </summary>
    public class ShopDashboardViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Products list
        public List<ShopProductViewModel> Products { get; set; } = new();

        // Statistics
        public int TotalProducts { get; set; }
        public int ActiveProducts { get; set; }
        public int PendingProducts => Products.Count(p => p.Status == "pending");
        public int DraftProducts => Products.Count(p => p.Status == "draft");

        // Helper properties
        public string StatusBadgeClass =>
            Status switch
            {
                "Active" => "badge-success",
                "Pending" => "badge-warning",
                "Inactive" => "badge-secondary",
                _ => "badge-info",
            };

        public string StatusDisplayName =>
            Status switch
            {
                "Active" => "Đang hoạt động",
                "Pending" => "Chờ duyệt",
                "Inactive" => "Ngừng hoạt động",
                _ => Status,
            };
    }

    /// <summary>
    /// ViewModel cho sản phẩm trong Shop Dashboard
    /// </summary>
    public class ShopProductViewModel
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

    /// <summary>
    /// ViewModel cho trang Shop Detail (public view)
    /// </summary>
    public class ShopDetailViewModel
    {
        public Guid Id { get; set; }
        public string ShopName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }

        // Products list
        public List<ShopProductViewModel> Products { get; set; } = new();

        public int ProductCount => Products.Count;
    }

    /// <summary>
    /// ViewModel cho trang Statistics của Shop
    /// </summary>
    public class ShopStatisticsViewModel
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
        public List<DailySalesViewModel> DailySales { get; set; } = new();

        // Top sản phẩm bán chạy
        public List<ProductRevenueViewModel> TopSellingProducts { get; set; } = new();

        // Thống kê doanh thu theo sản phẩm
        public List<ProductRevenueViewModel> ProductRevenues { get; set; } = new();

        // Đơn hàng gần đây
        public List<ShopRecentOrderViewModel> RecentOrders { get; set; } = new();
    }

    /// <summary>
    /// ViewModel doanh thu theo sản phẩm
    /// </summary>
    public class ProductRevenueViewModel
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

    /// <summary>
    /// ViewModel đơn hàng gần đây của Shop
    /// </summary>
    public class ShopRecentOrderViewModel
    {
        public Guid Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public string? CustomerName { get; set; }
        public string? ShopName { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// ViewModel thống kê doanh thu theo ngày
    /// </summary>
    public class DailySalesViewModel
    {
        public DateTime Date { get; set; }
        public string DateLabel { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
    }
}
