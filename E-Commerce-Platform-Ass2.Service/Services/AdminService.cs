using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Models;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ Admin
    /// </summary>
    public class AdminService : IAdminService
    {
        private readonly IShopRepository _shopRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IOrderRepository _orderRepository;

        public AdminService(
            IShopRepository shopRepository,
            IProductRepository productRepository,
            IProductVariantRepository productVariantRepository,
            IUserRepository userRepository,
            ICategoryRepository categoryRepository,
            IOrderRepository orderRepository
        )
        {
            _shopRepository = shopRepository;
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _orderRepository = orderRepository;
        }

        #region Shop Management

        public async Task<ServiceResult<List<ShopDto>>> GetAllShopsAsync()
        {
            var shops = await _shopRepository.GetAllAsync();
            var shopDtos = new List<ShopDto>();

            foreach (var shop in shops)
            {
                var owner = await _userRepository.GetByIdAsync(shop.UserId);
                var products = await _productRepository.GetByShopIdAsync(shop.Id);

                shopDtos.Add(
                    new ShopDto
                    {
                        Id = shop.Id,
                        UserId = shop.UserId,
                        ShopName = shop.ShopName,
                        Description = shop.Description,
                        Status = shop.Status,
                        CreatedAt = shop.CreatedAt,
                        OwnerName = owner?.Name,
                        OwnerEmail = owner?.Email,
                        ProductCount = products.Count(),
                    }
                );
            }

            return ServiceResult<List<ShopDto>>.Success(shopDtos);
        }

        public async Task<ServiceResult<List<ShopDto>>> GetShopsByStatusAsync(string status)
        {
            var shops = await _shopRepository.GetByStatus(status);
            var shopDtos = new List<ShopDto>();

            foreach (var shop in shops)
            {
                var owner = await _userRepository.GetByIdAsync(shop.UserId);
                var products = await _productRepository.GetByShopIdAsync(shop.Id);

                shopDtos.Add(
                    new ShopDto
                    {
                        Id = shop.Id,
                        UserId = shop.UserId,
                        ShopName = shop.ShopName,
                        Description = shop.Description,
                        Status = shop.Status,
                        CreatedAt = shop.CreatedAt,
                        OwnerName = owner?.Name,
                        OwnerEmail = owner?.Email,
                        ProductCount = products.Count(),
                    }
                );
            }

            return ServiceResult<List<ShopDto>>.Success(shopDtos);
        }

        public async Task<ServiceResult<ShopDetailDto>> GetShopDetailAsync(Guid shopId)
        {
            var shop = await _shopRepository.GetByIdAsync(shopId);
            if (shop == null)
            {
                return ServiceResult<ShopDetailDto>.Failure("Shop không tồn tại.");
            }

            var owner = await _userRepository.GetByIdAsync(shop.UserId);
            var products = await _productRepository.GetByShopIdAsync(shop.Id);

            var dto = new ShopDetailDto
            {
                Id = shop.Id,
                UserId = shop.UserId,
                ShopName = shop.ShopName,
                Description = shop.Description,
                Status = shop.Status,
                CreatedAt = shop.CreatedAt,
                OwnerName = owner?.Name,
                OwnerEmail = owner?.Email,
                ProductCount = products.Count(),
                Products = products
                    .Select(p => new ProductDto
                    {
                        Id = p.Id,
                        ShopId = p.ShopId,
                        CategoryId = p.CategoryId,
                        Name = p.Name,
                        Description = p.Description,
                        BasePrice = p.BasePrice,
                        Status = p.Status,
                        AvgRating = p.AvgRating,
                        ImageUrl = p.ImageUrl,
                        CreatedAt = p.CreatedAt,
                        ShopName = shop.ShopName,
                    })
                    .ToList(),
            };

            return ServiceResult<ShopDetailDto>.Success(dto);
        }

        public async Task<ServiceResult> ApproveShopAsync(Guid shopId)
        {
            var shop = await _shopRepository.GetByIdAsync(shopId);
            if (shop == null)
            {
                return ServiceResult.Failure("Shop không tồn tại.");
            }

            shop.Status = "Active";
            await _shopRepository.UpdateAsync(shop);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> RejectShopAsync(Guid shopId, string? reason = null)
        {
            var shop = await _shopRepository.GetByIdAsync(shopId);
            if (shop == null)
            {
                return ServiceResult.Failure("Shop không tồn tại.");
            }

            shop.Status = "Inactive";
            await _shopRepository.UpdateAsync(shop);

            return ServiceResult.Success();
        }

        #endregion

        #region Product Approval

        public async Task<ServiceResult<List<ProductDto>>> GetPendingProductsAsync()
        {
            return await GetProductsByStatusAsync("pending");
        }

        public async Task<ServiceResult<List<ProductDto>>> GetAllProductsAsync()
        {
            var allProducts = await _productRepository.GetAllAsync();
            // Admin chỉ thấy sản phẩm đã submit (không thấy draft)
            var products = allProducts.Where(p => p.Status != "draft");
            var productDtos = new List<ProductDto>();

            foreach (var product in products)
            {
                var shop = await _shopRepository.GetByIdAsync(product.ShopId);
                var category = await _categoryRepository.GetByIdAsync(product.CategoryId);

                productDtos.Add(
                    new ProductDto
                    {
                        Id = product.Id,
                        ShopId = product.ShopId,
                        CategoryId = product.CategoryId,
                        Name = product.Name,
                        Description = product.Description,
                        BasePrice = product.BasePrice,
                        Status = product.Status,
                        AvgRating = product.AvgRating,
                        ImageUrl = product.ImageUrl,
                        CreatedAt = product.CreatedAt,
                        ShopName = shop?.ShopName,
                        CategoryName = category?.Name,
                    }
                );
            }

            return ServiceResult<List<ProductDto>>.Success(productDtos);
        }

        public async Task<ServiceResult<List<ProductDto>>> GetProductsByStatusAsync(string status)
        {
            var allProducts = await _productRepository.GetAllAsync();
            var products = allProducts.Where(p => p.Status == status);
            var productDtos = new List<ProductDto>();

            foreach (var product in products)
            {
                var shop = await _shopRepository.GetByIdAsync(product.ShopId);
                var category = await _categoryRepository.GetByIdAsync(product.CategoryId);

                productDtos.Add(
                    new ProductDto
                    {
                        Id = product.Id,
                        ShopId = product.ShopId,
                        CategoryId = product.CategoryId,
                        Name = product.Name,
                        Description = product.Description,
                        BasePrice = product.BasePrice,
                        Status = product.Status,
                        AvgRating = product.AvgRating,
                        ImageUrl = product.ImageUrl,
                        CreatedAt = product.CreatedAt,
                        ShopName = shop?.ShopName,
                        CategoryName = category?.Name,
                    }
                );
            }

            return ServiceResult<List<ProductDto>>.Success(productDtos);
        }

        public async Task<ServiceResult<ProductDetailDto>> GetProductForApprovalAsync(
            Guid productId
        )
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return ServiceResult<ProductDetailDto>.Failure("Sản phẩm không tồn tại.");
            }

            var variants = await _productVariantRepository.GetByProductIdAsync(productId);
            var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
            var shop = await _shopRepository.GetByIdAsync(product.ShopId);

            var dto = new ProductDetailDto
            {
                Id = product.Id,
                ShopId = product.ShopId,
                CategoryId = product.CategoryId,
                Name = product.Name,
                Description = product.Description,
                BasePrice = product.BasePrice,
                Status = product.Status,
                AvgRating = product.AvgRating,
                ImageUrl = product.ImageUrl,
                CreatedAt = product.CreatedAt,
                CategoryName = category?.Name,
                ShopName = shop?.ShopName,
                Variants = variants
                    .Select(v => new ProductVariantDto
                    {
                        Id = v.Id,
                        ProductId = v.ProductId,
                        VariantName = v.VariantName,
                        Price = v.Price,
                        Size = v.Size,
                        Color = v.Color,
                        Stock = v.Stock,
                        Sku = v.Sku,
                        Status = v.Status,
                        ImageUrl = v.ImageUrl,
                    })
                    .ToList(),
            };

            return ServiceResult<ProductDetailDto>.Success(dto);
        }

        public async Task<ServiceResult> ApproveProductAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return ServiceResult.Failure("Sản phẩm không tồn tại.");
            }

            if (product.Status != "pending")
            {
                return ServiceResult.Failure(
                    "Chỉ có thể duyệt sản phẩm đang ở trạng thái chờ duyệt."
                );
            }

            product.Status = "active";
            await _productRepository.UpdateAsync(product);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> RejectProductAsync(Guid productId, string? reason = null)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return ServiceResult.Failure("Sản phẩm không tồn tại.");
            }

            if (product.Status != "pending")
            {
                return ServiceResult.Failure(
                    "Chỉ có thể từ chối sản phẩm đang ở trạng thái chờ duyệt."
                );
            }

            product.Status = "rejected";
            await _productRepository.UpdateAsync(product);

            return ServiceResult.Success();
        }

        #endregion

        #region Statistics

        public async Task<AdminDashboardDto> GetDashboardStatisticsAsync()
        {
            var allShops = (await _shopRepository.GetAllAsync()).ToList();
            var allProducts = (await _productRepository.GetAllAsync()).ToList();
            var allUsers = (await _userRepository.GetAllAsync()).ToList();
            var allOrders = (await _orderRepository.GetAllWithDetailsAsync()).ToList();

            // Time boundaries
            var now = DateTime.Now;
            var startOfWeek = now.AddDays(-(int)now.DayOfWeek).Date;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            // Completed statuses (case-insensitive)
            var completedStatuses = new[] { "completed", "delivered" };
            var cancelledStatuses = new[] { "cancelled", "canceled" };

            var dto = new AdminDashboardDto
            {
                // Shops
                TotalShops = allShops.Count,
                ActiveShops = allShops.Count(s => s.Status == "Active"),
                PendingShops = allShops.Count(s => s.Status == "Pending"),
                InactiveShops = allShops.Count(s => s.Status == "Inactive"),

                // Products
                TotalProducts = allProducts.Count,
                ActiveProducts = allProducts.Count(p => p.Status == "active"),
                PendingProducts = allProducts.Count(p => p.Status == "pending"),
                DraftProducts = allProducts.Count(p => p.Status == "draft"),
                RejectedProducts = allProducts.Count(p => p.Status == "rejected"),

                // Users
                TotalUsers = allUsers.Count,

                // Orders Total
                TotalOrders = allOrders.Count,
                TotalRevenue = allOrders
                    .Where(o => completedStatuses.Contains(o.Status?.ToLower() ?? ""))
                    .Sum(o => o.TotalAmount),

                // Weekly Statistics
                WeeklyOrders = allOrders.Count(o => o.CreatedAt >= startOfWeek),
                WeeklyRevenue = allOrders
                    .Where(o =>
                        o.CreatedAt >= startOfWeek
                        && completedStatuses.Contains(o.Status?.ToLower() ?? "")
                    )
                    .Sum(o => o.TotalAmount),
                WeeklyCompletedOrders = allOrders.Count(o =>
                    o.CreatedAt >= startOfWeek
                    && completedStatuses.Contains(o.Status?.ToLower() ?? "")
                ),
                WeeklyCancelledOrders = allOrders.Count(o =>
                    o.CreatedAt >= startOfWeek
                    && cancelledStatuses.Contains(o.Status?.ToLower() ?? "")
                ),

                // Monthly Statistics
                MonthlyOrders = allOrders.Count(o => o.CreatedAt >= startOfMonth),
                MonthlyRevenue = allOrders
                    .Where(o =>
                        o.CreatedAt >= startOfMonth
                        && completedStatuses.Contains(o.Status?.ToLower() ?? "")
                    )
                    .Sum(o => o.TotalAmount),
                MonthlyCompletedOrders = allOrders.Count(o =>
                    o.CreatedAt >= startOfMonth
                    && completedStatuses.Contains(o.Status?.ToLower() ?? "")
                ),
                MonthlyCancelledOrders = allOrders.Count(o =>
                    o.CreatedAt >= startOfMonth
                    && cancelledStatuses.Contains(o.Status?.ToLower() ?? "")
                ),

                // Initialize lists
                RecentShops = new List<ShopDto>(),
                RecentPendingProducts = new List<ProductDto>(),
                RecentOrders = new List<RecentOrderDto>(),
                TopShopsByRevenue = new List<ShopSalesStatDto>(),
                ShopWeeklyStats = new List<ShopSalesStatDto>(),
                ShopMonthlyStats = new List<ShopSalesStatDto>(),
                DailySalesStats = new List<DailySalesDto>(),
            };

            // Calculate Daily Sales for last 7 days (for chart)
            for (int i = 6; i >= 0; i--)
            {
                var date = now.AddDays(-i).Date;
                var dayOrders = allOrders.Where(o => o.CreatedAt.Date == date).ToList();
                dto.DailySalesStats.Add(
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

            // Calculate Shop Statistics
            var shopStats = new Dictionary<Guid, ShopSalesStatDto>();
            foreach (var shop in allShops.Where(s => s.Status == "Active"))
            {
                var owner = await _userRepository.GetByIdAsync(shop.UserId);
                var shopOrders = allOrders
                    .Where(o =>
                        o.OrderItems.Any(oi => oi.ProductVariant?.Product?.ShopId == shop.Id)
                    )
                    .ToList();

                var stat = new ShopSalesStatDto
                {
                    ShopId = shop.Id,
                    ShopName = shop.ShopName,
                    OwnerName = owner?.Name,
                    TotalOrders = shopOrders.Count,
                    CompletedOrders = shopOrders.Count(o =>
                        completedStatuses.Contains(o.Status?.ToLower() ?? "")
                    ),
                    CancelledOrders = shopOrders.Count(o =>
                        cancelledStatuses.Contains(o.Status?.ToLower() ?? "")
                    ),
                    PendingOrders = shopOrders.Count(o =>
                        !completedStatuses.Contains(o.Status?.ToLower() ?? "")
                        && !cancelledStatuses.Contains(o.Status?.ToLower() ?? "")
                    ),
                    TotalRevenue = shopOrders.Sum(o => o.TotalAmount),
                    CompletedRevenue = shopOrders
                        .Where(o => completedStatuses.Contains(o.Status?.ToLower() ?? ""))
                        .Sum(o => o.TotalAmount),
                    TotalProductsSold = shopOrders
                        .SelectMany(o => o.OrderItems)
                        .Where(oi => oi.ProductVariant?.Product?.ShopId == shop.Id)
                        .Sum(oi => oi.Quantity),
                };
                shopStats[shop.Id] = stat;
            }

            // Top shops by revenue (all time)
            dto.TopShopsByRevenue = shopStats
                .Values.OrderByDescending(s => s.CompletedRevenue)
                .Take(10)
                .ToList();

            // Shop Weekly Stats
            foreach (var shop in allShops.Where(s => s.Status == "Active"))
            {
                var owner = await _userRepository.GetByIdAsync(shop.UserId);
                var weeklyOrders = allOrders
                    .Where(o =>
                        o.CreatedAt >= startOfWeek
                        && o.OrderItems.Any(oi => oi.ProductVariant?.Product?.ShopId == shop.Id)
                    )
                    .ToList();

                if (weeklyOrders.Any())
                {
                    dto.ShopWeeklyStats.Add(
                        new ShopSalesStatDto
                        {
                            ShopId = shop.Id,
                            ShopName = shop.ShopName,
                            OwnerName = owner?.Name,
                            TotalOrders = weeklyOrders.Count,
                            CompletedOrders = weeklyOrders.Count(o =>
                                completedStatuses.Contains(o.Status?.ToLower() ?? "")
                            ),
                            CancelledOrders = weeklyOrders.Count(o =>
                                cancelledStatuses.Contains(o.Status?.ToLower() ?? "")
                            ),
                            TotalRevenue = weeklyOrders.Sum(o => o.TotalAmount),
                            CompletedRevenue = weeklyOrders
                                .Where(o => completedStatuses.Contains(o.Status?.ToLower() ?? ""))
                                .Sum(o => o.TotalAmount),
                            TotalProductsSold = weeklyOrders
                                .SelectMany(o => o.OrderItems)
                                .Where(oi => oi.ProductVariant?.Product?.ShopId == shop.Id)
                                .Sum(oi => oi.Quantity),
                        }
                    );
                }
            }
            dto.ShopWeeklyStats = dto
                .ShopWeeklyStats.OrderByDescending(s => s.CompletedRevenue)
                .Take(10)
                .ToList();

            // Shop Monthly Stats
            foreach (var shop in allShops.Where(s => s.Status == "Active"))
            {
                var owner = await _userRepository.GetByIdAsync(shop.UserId);
                var monthlyOrders = allOrders
                    .Where(o =>
                        o.CreatedAt >= startOfMonth
                        && o.OrderItems.Any(oi => oi.ProductVariant?.Product?.ShopId == shop.Id)
                    )
                    .ToList();

                if (monthlyOrders.Any())
                {
                    dto.ShopMonthlyStats.Add(
                        new ShopSalesStatDto
                        {
                            ShopId = shop.Id,
                            ShopName = shop.ShopName,
                            OwnerName = owner?.Name,
                            TotalOrders = monthlyOrders.Count,
                            CompletedOrders = monthlyOrders.Count(o =>
                                completedStatuses.Contains(o.Status?.ToLower() ?? "")
                            ),
                            CancelledOrders = monthlyOrders.Count(o =>
                                cancelledStatuses.Contains(o.Status?.ToLower() ?? "")
                            ),
                            TotalRevenue = monthlyOrders.Sum(o => o.TotalAmount),
                            CompletedRevenue = monthlyOrders
                                .Where(o => completedStatuses.Contains(o.Status?.ToLower() ?? ""))
                                .Sum(o => o.TotalAmount),
                            TotalProductsSold = monthlyOrders
                                .SelectMany(o => o.OrderItems)
                                .Where(oi => oi.ProductVariant?.Product?.ShopId == shop.Id)
                                .Sum(oi => oi.Quantity),
                        }
                    );
                }
            }
            dto.ShopMonthlyStats = dto
                .ShopMonthlyStats.OrderByDescending(s => s.CompletedRevenue)
                .Take(10)
                .ToList();

            // Get recent shops
            var recentShops = allShops.OrderByDescending(s => s.CreatedAt).Take(5);
            foreach (var shop in recentShops)
            {
                var owner = await _userRepository.GetByIdAsync(shop.UserId);
                var products = await _productRepository.GetByShopIdAsync(shop.Id);
                dto.RecentShops.Add(
                    new ShopDto
                    {
                        Id = shop.Id,
                        UserId = shop.UserId,
                        ShopName = shop.ShopName,
                        Description = shop.Description,
                        Status = shop.Status,
                        CreatedAt = shop.CreatedAt,
                        OwnerName = owner?.Name,
                        OwnerEmail = owner?.Email,
                        ProductCount = products.Count(),
                    }
                );
            }

            // Get recent pending products (last 5)
            var recentPendingProducts = allProducts
                .Where(p => p.Status == "pending")
                .OrderByDescending(p => p.CreatedAt)
                .Take(5);

            foreach (var product in recentPendingProducts)
            {
                var shop = await _shopRepository.GetByIdAsync(product.ShopId);
                var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
                dto.RecentPendingProducts.Add(
                    new ProductDto
                    {
                        Id = product.Id,
                        ShopId = product.ShopId,
                        CategoryId = product.CategoryId,
                        Name = product.Name,
                        Description = product.Description,
                        BasePrice = product.BasePrice,
                        Status = product.Status,
                        AvgRating = product.AvgRating,
                        ImageUrl = product.ImageUrl,
                        CreatedAt = product.CreatedAt,
                        ShopName = shop?.ShopName,
                        CategoryName = category?.Name,
                    }
                );
            }

            // Get recent orders (last 5)
            var recentOrders = allOrders.Take(5);
            foreach (var order in recentOrders)
            {
                var shopName = order
                    .OrderItems.FirstOrDefault()
                    ?.ProductVariant?.Product?.Shop?.ShopName;
                if (string.IsNullOrEmpty(shopName) && order.OrderItems.Any())
                {
                    var firstItem = order.OrderItems.First();
                    if (firstItem.ProductVariant?.Product != null)
                    {
                        var shop = await _shopRepository.GetByIdAsync(
                            firstItem.ProductVariant.Product.ShopId
                        );
                        shopName = shop?.ShopName;
                    }
                }

                dto.RecentOrders.Add(
                    new RecentOrderDto
                    {
                        Id = order.Id,
                        OrderCode = order.Id.ToString().Substring(0, 8).ToUpper(),
                        CustomerName = order.User?.Name ?? "N/A",
                        ShopName = shopName,
                        TotalAmount = order.TotalAmount,
                        Status = order.Status ?? "Unknown",
                        CreatedAt = order.CreatedAt,
                    }
                );
            }

            return dto;
        }

        #endregion

        #region Category Management

        public async Task<ServiceResult<List<CategoryDto>>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            var allProducts = await _productRepository.GetAllAsync();

            var categoryDtos = categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Status = c.Status,
                    ProductCount = allProducts.Count(p => p.CategoryId == c.Id),
                })
                .ToList();

            return ServiceResult<List<CategoryDto>>.Success(categoryDtos);
        }

        public async Task<ServiceResult<CategoryDto>> GetCategoryByIdAsync(Guid categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return ServiceResult<CategoryDto>.Failure("Danh mục không tồn tại.");
            }

            var allProducts = await _productRepository.GetAllAsync();
            var dto = new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Status = category.Status,
                ProductCount = allProducts.Count(p => p.CategoryId == category.Id),
            };

            return ServiceResult<CategoryDto>.Success(dto);
        }

        public async Task<ServiceResult<Guid>> CreateCategoryAsync(CreateCategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return ServiceResult<Guid>.Failure("Tên danh mục không được để trống.");
            }

            // Kiểm tra tên đã tồn tại chưa
            var exists = await _categoryRepository.ExistsByName(dto.Name);
            if (exists)
            {
                return ServiceResult<Guid>.Failure("Tên danh mục đã tồn tại.");
            }

            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = dto.Name.Trim(),
                Status = dto.Status ?? "Active",
            };

            await _categoryRepository.AddAsync(category);

            return ServiceResult<Guid>.Success(category.Id);
        }

        public async Task<ServiceResult> UpdateCategoryAsync(Guid categoryId, UpdateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return ServiceResult.Failure("Danh mục không tồn tại.");
            }

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return ServiceResult.Failure("Tên danh mục không được để trống.");
            }

            // Kiểm tra tên đã tồn tại ở category khác chưa
            var existingCategory = await _categoryRepository.GetAllAsync();
            if (existingCategory.Any(c => c.Name == dto.Name.Trim() && c.Id != categoryId))
            {
                return ServiceResult.Failure("Tên danh mục đã tồn tại.");
            }

            category.Name = dto.Name.Trim();
            category.Status = dto.Status;

            await _categoryRepository.UpdateAsync(category);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> DeleteCategoryAsync(Guid categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                return ServiceResult.Failure("Danh mục không tồn tại.");
            }

            // Kiểm tra có sản phẩm nào đang sử dụng category này không
            var products = await _productRepository.GetByCategoryIdAsync(categoryId);
            if (products.Any())
            {
                return ServiceResult.Failure(
                    $"Không thể xóa danh mục đang có {products.Count()} sản phẩm."
                );
            }

            await _categoryRepository.DeleteAsync(category);

            return ServiceResult.Success();
        }

        #endregion
    }
}
