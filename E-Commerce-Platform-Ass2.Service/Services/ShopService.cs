using System;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class ShopService : IShopService
    {
        private readonly IShopRepository _shopRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public ShopService(
            IShopRepository shopRepository,
            IOrderRepository orderRepository,
            IProductRepository productRepository
        )
        {
            _shopRepository = shopRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<Shop?> GetShopByUserIdAsync(Guid userId)
        {
            return await _shopRepository.GetByUserIdAsync(userId);
        }

        public async Task<Shop?> GetShopByIdAsync(Guid shopId)
        {
            return await _shopRepository.GetByIdAsync(shopId);
        }

        public async Task<ShopDto?> GetShopDtoByUserIdAsync(Guid userId)
        {
            var shop = await _shopRepository.GetByUserIdAsync(userId);
            return shop == null ? null : MapToDto(shop);
        }

        public async Task<ShopDto?> GetShopDtoByIdAsync(Guid shopId)
        {
            var shop = await _shopRepository.GetByIdAsync(shopId);
            return shop == null ? null : MapToDto(shop);
        }

        private static ShopDto MapToDto(Shop shop)
        {
            return new ShopDto
            {
                Id = shop.Id,
                UserId = shop.UserId,
                ShopName = shop.ShopName,
                Description = shop.Description,
                Status = shop.Status,
                CreatedAt = shop.CreatedAt,
                OwnerName = shop.User?.Name,
                OwnerEmail = shop.User?.Email,
            };
        }

        public async Task<bool> UserHasShopAsync(Guid userId)
        {
            return await _shopRepository.ExistsByUserId(userId);
        }

        public async Task<bool> ShopNameExistsAsync(string shopName)
        {
            return await _shopRepository.ExistsByShopName(shopName);
        }

        public async Task<Shop> RegisterShopAsync(Guid userId, string shopName, string description)
        {
            var shop = new Shop
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ShopName = shopName.Trim(),
                Description = description.Trim(),
                Status = "Pending", // Chờ phê duyệt
                CreatedAt = DateTime.UtcNow,
            };

            return await _shopRepository.AddAsync(shop);
        }

        public async Task<ShopStatisticsDto> GetShopStatisticsAsync(Guid shopId)
        {
            var shop = await _shopRepository.GetByIdAsync(shopId);
            if (shop == null)
            {
                return new ShopStatisticsDto();
            }

            // Lấy tất cả đơn hàng của shop
            var orders = (await _orderRepository.GetByShopIdAsync(shopId)).ToList();

            // Lấy tất cả sản phẩm của shop
            var products = (await _productRepository.GetByShopIdAsync(shopId)).ToList();

            // Time boundaries
            var now = DateTime.Now;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek).Date;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            // Completed statuses (case-insensitive)
            var completedStatuses = new[] { "completed", "delivered" };
            var cancelledStatuses = new[] { "cancelled", "canceled" };

            var dto = new ShopStatisticsDto
            {
                ShopId = shopId,
                ShopName = shop.ShopName,

                // Thống kê sản phẩm
                TotalProducts = products.Count,
                ActiveProducts = products.Count(p => p.Status?.ToLower() == "active"),
                PendingProducts = products.Count(p => p.Status?.ToLower() == "pending"),

                // Thống kê tổng quan
                TotalOrders = orders.Count,
                TotalRevenue = orders.Sum(o => o.TotalAmount),
                CompletedRevenue = orders
                    .Where(o => completedStatuses.Contains(o.Status?.ToLower() ?? ""))
                    .Sum(o => o.TotalAmount),
                TotalProductsSold = orders
                    .SelectMany(o => o.OrderItems)
                    .Where(oi => oi.ProductVariant?.Product?.ShopId == shopId)
                    .Sum(oi => oi.Quantity),

                // Thống kê tuần này
                WeeklyOrders = orders.Count(o => o.CreatedAt >= startOfWeek),
                WeeklyRevenue = orders
                    .Where(o =>
                        o.CreatedAt >= startOfWeek
                        && completedStatuses.Contains(o.Status?.ToLower() ?? "")
                    )
                    .Sum(o => o.TotalAmount),
                WeeklyCompletedOrders = orders.Count(o =>
                    o.CreatedAt >= startOfWeek
                    && completedStatuses.Contains(o.Status?.ToLower() ?? "")
                ),
                WeeklyCancelledOrders = orders.Count(o =>
                    o.CreatedAt >= startOfWeek
                    && cancelledStatuses.Contains(o.Status?.ToLower() ?? "")
                ),
                WeeklyPendingOrders = orders.Count(o =>
                    o.CreatedAt >= startOfWeek
                    && !completedStatuses.Contains(o.Status?.ToLower() ?? "")
                    && !cancelledStatuses.Contains(o.Status?.ToLower() ?? "")
                ),
                WeeklyProductsSold = orders
                    .Where(o => o.CreatedAt >= startOfWeek)
                    .SelectMany(o => o.OrderItems)
                    .Where(oi => oi.ProductVariant?.Product?.ShopId == shopId)
                    .Sum(oi => oi.Quantity),

                // Thống kê tháng này
                MonthlyOrders = orders.Count(o => o.CreatedAt >= startOfMonth),
                MonthlyRevenue = orders
                    .Where(o =>
                        o.CreatedAt >= startOfMonth
                        && completedStatuses.Contains(o.Status?.ToLower() ?? "")
                    )
                    .Sum(o => o.TotalAmount),
                MonthlyCompletedOrders = orders.Count(o =>
                    o.CreatedAt >= startOfMonth
                    && completedStatuses.Contains(o.Status?.ToLower() ?? "")
                ),
                MonthlyCancelledOrders = orders.Count(o =>
                    o.CreatedAt >= startOfMonth
                    && cancelledStatuses.Contains(o.Status?.ToLower() ?? "")
                ),
                MonthlyPendingOrders = orders.Count(o =>
                    o.CreatedAt >= startOfMonth
                    && !completedStatuses.Contains(o.Status?.ToLower() ?? "")
                    && !cancelledStatuses.Contains(o.Status?.ToLower() ?? "")
                ),
                MonthlyProductsSold = orders
                    .Where(o => o.CreatedAt >= startOfMonth)
                    .SelectMany(o => o.OrderItems)
                    .Where(oi => oi.ProductVariant?.Product?.ShopId == shopId)
                    .Sum(oi => oi.Quantity),

                // Initialize lists
                DailySales = new List<DailySalesDto>(),
                TopSellingProducts = new List<ProductRevenueDto>(),
                ProductRevenues = new List<ProductRevenueDto>(),
                RecentOrders = new List<RecentOrderDto>(),
            };

            // Daily Sales for last 7 days (for chart)
            for (int i = 6; i >= 0; i--)
            {
                var date = now.AddDays(-i).Date;
                var dayOrders = orders.Where(o => o.CreatedAt.Date == date).ToList();
                dto.DailySales.Add(
                    new DailySalesDto
                    {
                        Date = date,
                        DateLabel = date.ToString("dd/MM"),
                        OrderCount = dayOrders.Count,
                        Revenue = dayOrders
                            .Where(o => completedStatuses.Contains(o.Status?.ToLower() ?? ""))
                            .Sum(o => o.TotalAmount),
                    }
                );
            }

            // Product Revenues - Thống kê doanh thu theo từng sản phẩm
            var productStats = new Dictionary<Guid, ProductRevenueDto>();

            foreach (var product in products)
            {
                var productOrderItems = orders
                    .SelectMany(o => o.OrderItems)
                    .Where(oi => oi.ProductVariant?.Product?.Id == product.Id)
                    .ToList();

                var weeklyItems = orders
                    .Where(o => o.CreatedAt >= startOfWeek)
                    .SelectMany(o => o.OrderItems)
                    .Where(oi => oi.ProductVariant?.Product?.Id == product.Id)
                    .ToList();

                var monthlyItems = orders
                    .Where(o => o.CreatedAt >= startOfMonth)
                    .SelectMany(o => o.OrderItems)
                    .Where(oi => oi.ProductVariant?.Product?.Id == product.Id)
                    .ToList();

                productStats[product.Id] = new ProductRevenueDto
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    ImageUrl = product.ImageUrl,
                    TotalQuantitySold = productOrderItems.Sum(oi => oi.Quantity),
                    TotalRevenue = productOrderItems.Sum(oi => oi.Price * oi.Quantity),
                    OrderCount = productOrderItems.Select(oi => oi.OrderId).Distinct().Count(),
                    WeeklyQuantitySold = weeklyItems.Sum(oi => oi.Quantity),
                    WeeklyRevenue = weeklyItems.Sum(oi => oi.Price * oi.Quantity),
                    MonthlyQuantitySold = monthlyItems.Sum(oi => oi.Quantity),
                    MonthlyRevenue = monthlyItems.Sum(oi => oi.Price * oi.Quantity),
                };
            }

            // Top selling products (by quantity)
            dto.TopSellingProducts = productStats
                .Values.OrderByDescending(p => p.TotalQuantitySold)
                .Take(5)
                .ToList();

            // All product revenues (sorted by revenue)
            dto.ProductRevenues = productStats
                .Values.OrderByDescending(p => p.TotalRevenue)
                .ToList();

            // Recent orders (last 5)
            var recentOrders = orders.Take(5);
            foreach (var order in recentOrders)
            {
                dto.RecentOrders.Add(
                    new RecentOrderDto
                    {
                        Id = order.Id,
                        OrderCode = order.Id.ToString().Substring(0, 8).ToUpper(),
                        CustomerName = order.User?.Name ?? "N/A",
                        TotalAmount = order.TotalAmount,
                        Status = order.Status ?? "Unknown",
                        CreatedAt = order.CreatedAt,
                    }
                );
            }

            return dto;
        }
    }
}
