using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Models;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ sản phẩm
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IShopRepository _shopRepository;

        public ProductService(
            IProductRepository productRepository,
            IProductVariantRepository productVariantRepository,
            ICategoryRepository categoryRepository,
            IShopRepository shopRepository
        )
        {
            _productRepository = productRepository;
            _productVariantRepository = productVariantRepository;
            _categoryRepository = categoryRepository;
            _shopRepository = shopRepository;
        }

        public async Task<List<Product>> GetAllProductAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products.ToList();
        }

        /// <summary>
        /// Lấy tất cả sản phẩm dưới dạng DTO (dùng cho Home page)
        /// </summary>
        public async Task<List<ProductDto>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();
            return products
                .Where(p => p.Status == "active")
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => MapToDto(p, p.Shop?.ShopName))
                .ToList();
        }

        /// <summary>
        /// Lấy chi tiết sản phẩm bao gồm variants dưới dạng DTO (dùng cho Product Detail page)
        /// </summary>
        public async Task<ProductDetailDto?> GetProductDetailDtoAsync(Guid productId)
        {
            var product = await _productRepository.GetProductWithVariantsAsync(productId);
            if (product == null)
                return null;

            return new ProductDetailDto
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
                CategoryName = product.Category?.Name,
                ShopName = product.Shop?.ShopName,
                Variants =
                    product
                        .ProductVariants?.Select(v => new ProductVariantDto
                        {
                            Id = v.Id,
                            VariantName = v.VariantName,
                            Price = v.Price,
                            Size = v.Size,
                            Color = v.Color,
                            Stock = v.Stock,
                            Sku = v.Sku,
                            Status = v.Status,
                            ImageUrl = v.ImageUrl,
                        })
                        .ToList() ?? new List<ProductVariantDto>(),
            };
        }

        public async Task<Product?> GetProductWithVariantsAsync(Guid productId)
        {
            var product = await _productRepository.GetProductWithVariantsAsync(productId);
            return product;
        }

        public async Task<ServiceResult<Guid>> CreateProductAsync(CreateProductDto dto)
        {
            // Validate Shop exists
            var shop = await _shopRepository.GetByIdAsync(dto.ShopId);
            if (shop == null)
            {
                return ServiceResult<Guid>.Failure("Shop không tồn tại.");
            }

            // Validate Category exists
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
            {
                return ServiceResult<Guid>.Failure("Danh mục không tồn tại.");
            }

            // Validate Name
            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                return ServiceResult<Guid>.Failure("Tên sản phẩm không được để trống.");
            }

            // Validate Price
            if (dto.BasePrice < 0)
            {
                return ServiceResult<Guid>.Failure("Giá sản phẩm phải lớn hơn hoặc bằng 0.");
            }

            // Create product
            var product = new Product
            {
                Id = Guid.NewGuid(),
                ShopId = dto.ShopId,
                CategoryId = dto.CategoryId,
                Name = dto.Name.Trim(),
                Description = dto.Description?.Trim() ?? string.Empty,
                BasePrice = dto.BasePrice,
                Status = "draft", // Mặc định draft khi tạo, chờ shop submit và admin duyệt
                AvgRating = 0,
                ImageUrl = dto.ImageUrl?.Trim() ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
            };

            await _productRepository.AddAsync(product);

            return ServiceResult<Guid>.Success(product.Id);
        }

        /// <summary>
        /// Lấy danh sách sản phẩm theo ShopId
        /// </summary>
        public async Task<ServiceResult<List<ProductDto>>> GetByShopIdAsync(Guid shopId)
        {
            var shop = await _shopRepository.GetByIdAsync(shopId);
            if (shop == null)
            {
                return ServiceResult<List<ProductDto>>.Failure("Shop không tồn tại.");
            }

            var products = await _productRepository.GetByShopIdAsync(shopId);
            var productDtos = products.Select(p => MapToDto(p, shop.ShopName)).ToList();

            return ServiceResult<List<ProductDto>>.Success(productDtos);
        }

        public async Task<
            ServiceResult<(List<ProductDto> Products, int TotalCount)>
        > GetByShopIdPagedAsync(Guid shopId, int page, int pageSize)
        {
            var shop = await _shopRepository.GetByIdAsync(shopId);
            if (shop == null)
            {
                return ServiceResult<(List<ProductDto>, int)>.Failure("Shop không tồn tại.");
            }

            var productsQuery = await _productRepository.GetByShopIdAsync(shopId);
            var totalCount = productsQuery.Count();
            var products = productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => MapToDto(p, shop.ShopName))
                .ToList();

            return ServiceResult<(List<ProductDto>, int)>.Success((products, totalCount));
        }

        public async Task<
            ServiceResult<(int Total, int Active, int Inactive)>
        > GetProductCountsByShopIdAsync(Guid shopId)
        {
            var shop = await _shopRepository.GetByIdAsync(shopId);
            if (shop == null)
            {
                return ServiceResult<(int, int, int)>.Failure("Shop không tồn tại.");
            }

            var products = await _productRepository.GetByShopIdAsync(shopId);
            var total = products.Count();
            var active = products.Count(p => p.Status == "Active");
            var inactive = total - active;

            return ServiceResult<(int, int, int)>.Success((total, active, inactive));
        }

        /// <summary>
        /// Lấy sản phẩm theo Id
        /// </summary>
        public async Task<ServiceResult<ProductDto>> GetByIdAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return ServiceResult<ProductDto>.Failure("Sản phẩm không tồn tại.");
            }

            var dto = MapToDto(product);
            return ServiceResult<ProductDto>.Success(dto);
        }

        /// <summary>
        /// Lấy tất cả danh mục đang hoạt động (Active) - dùng cho dropdown khi tạo sản phẩm
        /// </summary>
        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories
                .Where(c => c.Status == "Active")
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Status = c.Status,
                })
                .ToList();
        }

        /// <summary>
        /// Lấy chi tiết sản phẩm bao gồm variants (dùng cho trang Edit)
        /// </summary>
        public async Task<ServiceResult<ProductDetailDto>> GetProductDetailAsync(
            Guid productId,
            Guid shopId
        )
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return ServiceResult<ProductDetailDto>.Failure("Sản phẩm không tồn tại.");
            }

            // Kiểm tra product thuộc shop của user
            if (product.ShopId != shopId)
            {
                return ServiceResult<ProductDetailDto>.Failure(
                    "Bạn không có quyền truy cập sản phẩm này."
                );
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

        /// <summary>
        /// Submit sản phẩm để admin duyệt (chuyển từ draft sang pending)
        /// </summary>
        public async Task<ServiceResult> SubmitProductAsync(Guid productId, Guid shopId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return ServiceResult.Failure("Sản phẩm không tồn tại.");
            }

            if (product.ShopId != shopId)
            {
                return ServiceResult.Failure("Bạn không có quyền submit sản phẩm này.");
            }

            if (product.Status != "draft")
            {
                return ServiceResult.Failure("Chỉ có thể submit sản phẩm ở trạng thái bản nháp.");
            }

            // Kiểm tra phải có ít nhất 1 variant
            var variants = await _productVariantRepository.GetByProductIdAsync(productId);
            if (!variants.Any())
            {
                return ServiceResult.Failure(
                    "Sản phẩm phải có ít nhất 1 biến thể trước khi submit."
                );
            }

            // Chuyển trạng thái sang pending để admin duyệt
            product.Status = "pending";
            await _productRepository.UpdateAsync(product);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> UpdateProductAsync(UpdateProductDto dto)
        {
            var product = await _productRepository.GetByIdAsync(dto.ProductId);
            if (product == null)
            {
                return ServiceResult.Failure("Sản phẩm không tồn tại.");
            }

            if (product.ShopId != dto.ShopId)
            {
                return ServiceResult.Failure("Bạn không có quyền cập nhật sản phẩm này.");
            }

            // Chỉ cho phép cập nhật khi ở trạng thái draft
            if (product.Status != "draft")
            {
                return ServiceResult.Failure("Chỉ có thể cập nhật sản phẩm ở trạng thái bản nháp.");
            }

            // Validate Category exists
            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId);
            if (category == null)
            {
                return ServiceResult.Failure("Danh mục không tồn tại.");
            }

            // Update product information
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.BasePrice = dto.BasePrice;
            product.ImageUrl = dto.ImageUrl;
            product.CategoryId = dto.CategoryId;

            await _productRepository.UpdateAsync(product);

            return ServiceResult.Success();
        }

        public async Task<ServiceResult> UnpublishProductAsync(Guid productId, Guid shopId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return ServiceResult.Failure("Sản phẩm không tồn tại.");
            }

            if (product.ShopId != shopId)
            {
                return ServiceResult.Failure("Bạn không có quyền thao tác với sản phẩm này.");
            }

            if (product.Status != "active")
            {
                return ServiceResult.Failure("Chỉ có thể gỡ sản phẩm đang hoạt động.");
            }

            product.Status = "draft";
            // Khi về draft, sản phẩm sẽ không hiển thị trên trang chủ nữa
            // Shop cần edit và submit lại để admin duyệt

            await _productRepository.UpdateAsync(product);

            return ServiceResult.Success();
        }

        private static ProductDto MapToDto(Product product, string? shopName = null)
        {
            return new ProductDto
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
                ShopName = shopName,
                CategoryName = product.Category?.Name,
            };
        }
    }
}
