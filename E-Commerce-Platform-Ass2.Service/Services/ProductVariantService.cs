using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Models;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    /// <summary>
    /// Service xử lý nghiệp vụ biến thể sản phẩm
    /// </summary>
    public class ProductVariantService : IProductVariantService
    {
        private readonly IProductVariantRepository _productVariantRepository;
        private readonly IProductRepository _productRepository;

        public ProductVariantService(
            IProductVariantRepository productVariantRepository,
            IProductRepository productRepository
        )
        {
            _productVariantRepository = productVariantRepository;
            _productRepository = productRepository;
        }

        /// <summary>
        /// Lấy danh sách biến thể theo ProductId
        /// </summary>
        public async Task<ServiceResult<List<ProductVariantDto>>> GetByProductIdAsync(
            Guid productId
        )
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                return ServiceResult<List<ProductVariantDto>>.Failure("Sản phẩm không tồn tại.");
            }

            var variants = await _productVariantRepository.GetByProductIdAsync(productId);
            var variantDtos = variants.Select(MapToDto).ToList();

            return ServiceResult<List<ProductVariantDto>>.Success(variantDtos);
        }

        /// <summary>
        /// Lấy biến thể theo Id
        /// </summary>
        public async Task<ServiceResult<ProductVariantDto>> GetByIdAsync(Guid variantId)
        {
            var variant = await _productVariantRepository.GetByIdAsync(variantId);
            if (variant == null)
            {
                return ServiceResult<ProductVariantDto>.Failure("Biến thể không tồn tại.");
            }

            return ServiceResult<ProductVariantDto>.Success(MapToDto(variant));
        }

        /// <summary>
        /// Thêm biến thể cho sản phẩm
        /// </summary>
        public async Task<ServiceResult<Guid>> AddVariantAsync(
            CreateProductVariantDto dto,
            Guid shopId
        )
        {
            var product = await _productRepository.GetByIdAsync(dto.ProductId);
            if (product == null)
            {
                return ServiceResult<Guid>.Failure("Sản phẩm không tồn tại.");
            }

            // Kiểm tra product thuộc shop của user
            if (product.ShopId != shopId)
            {
                return ServiceResult<Guid>.Failure(
                    "Bạn không có quyền thêm biến thể cho sản phẩm này."
                );
            }

            // Chỉ cho phép thêm variant khi product đang ở trạng thái draft
            if (product.Status != "draft")
            {
                return ServiceResult<Guid>.Failure(
                    "Chỉ có thể thêm biến thể khi sản phẩm ở trạng thái bản nháp."
                );
            }

            // Validate
            if (string.IsNullOrWhiteSpace(dto.VariantName))
            {
                return ServiceResult<Guid>.Failure("Tên biến thể không được để trống.");
            }

            if (dto.Price < 0)
            {
                return ServiceResult<Guid>.Failure("Giá biến thể phải lớn hơn hoặc bằng 0.");
            }

            if (dto.Stock < 0)
            {
                return ServiceResult<Guid>.Failure("Số lượng tồn kho phải lớn hơn hoặc bằng 0.");
            }

            var variant = new ProductVariant
            {
                Id = Guid.NewGuid(),
                ProductId = dto.ProductId,
                VariantName = dto.VariantName.Trim(),
                Price = dto.Price,
                Size = dto.Size?.Trim() ?? string.Empty,
                Color = dto.Color?.Trim() ?? string.Empty,
                Stock = dto.Stock,
                Sku = dto.Sku?.Trim() ?? string.Empty,
                Status = "active",
                ImageUrl = dto.ImageUrl?.Trim() ?? string.Empty,
            };

            await _productVariantRepository.AddAsync(variant);

            return ServiceResult<Guid>.Success(variant.Id);
        }

        /// <summary>
        /// Cập nhật biến thể
        /// </summary>
        public async Task<ServiceResult> UpdateVariantAsync(
            UpdateProductVariantDto dto,
            Guid shopId
        )
        {
            var variant = await _productVariantRepository.GetByIdAsync(dto.Id);
            if (variant == null)
            {
                return ServiceResult.Failure("Biến thể không tồn tại.");
            }

            var product = await _productRepository.GetByIdAsync(variant.ProductId);
            if (product == null || product.ShopId != shopId)
            {
                return ServiceResult.Failure("Bạn không có quyền cập nhật biến thể này.");
            }

            // Chỉ cho phép cập nhật variant khi product đang ở trạng thái draft
            if (product.Status != "draft")
            {
                return ServiceResult.Failure(
                    "Chỉ có thể cập nhật biến thể khi sản phẩm ở trạng thái bản nháp."
                );
            }

            // Validate
            if (string.IsNullOrWhiteSpace(dto.VariantName))
            {
                return ServiceResult.Failure("Tên biến thể không được để trống.");
            }

            if (dto.Price < 0)
            {
                return ServiceResult.Failure("Giá biến thể phải lớn hơn hoặc bằng 0.");
            }

            if (dto.Stock < 0)
            {
                return ServiceResult.Failure("Số lượng tồn kho phải lớn hơn hoặc bằng 0.");
            }

            // Update variant
            variant.VariantName = dto.VariantName.Trim();
            variant.Price = dto.Price;
            variant.Size = dto.Size?.Trim() ?? string.Empty;
            variant.Color = dto.Color?.Trim() ?? string.Empty;
            variant.Stock = dto.Stock;
            variant.Sku = dto.Sku?.Trim() ?? string.Empty;
            variant.ImageUrl = dto.ImageUrl?.Trim() ?? string.Empty;

            await _productVariantRepository.UpdateAsync(variant);

            return ServiceResult.Success();
        }

        /// <summary>
        /// Xóa biến thể
        /// </summary>
        public async Task<ServiceResult> DeleteVariantAsync(Guid variantId, Guid shopId)
        {
            var variant = await _productVariantRepository.GetByIdAsync(variantId);
            if (variant == null)
            {
                return ServiceResult.Failure("Biến thể không tồn tại.");
            }

            var product = await _productRepository.GetByIdAsync(variant.ProductId);
            if (product == null || product.ShopId != shopId)
            {
                return ServiceResult.Failure("Bạn không có quyền xóa biến thể này.");
            }

            // Chỉ cho phép xóa variant khi product đang ở trạng thái draft
            if (product.Status != "draft")
            {
                return ServiceResult.Failure(
                    "Chỉ có thể xóa biến thể khi sản phẩm ở trạng thái bản nháp."
                );
            }

            await _productVariantRepository.DeleteAsync(variant);

            return ServiceResult.Success();
        }

        /// <summary>
        /// Kiểm tra sản phẩm có biến thể hay không
        /// </summary>
        public async Task<bool> HasVariantsAsync(Guid productId)
        {
            var variants = await _productVariantRepository.GetByProductIdAsync(productId);
            return variants.Any();
        }

        private static ProductVariantDto MapToDto(ProductVariant variant)
        {
            return new ProductVariantDto
            {
                Id = variant.Id,
                ProductId = variant.ProductId,
                VariantName = variant.VariantName,
                Price = variant.Price,
                Size = variant.Size,
                Color = variant.Color,
                Stock = variant.Stock,
                Sku = variant.Sku,
                Status = variant.Status,
                ImageUrl = variant.ImageUrl,
            };
        }
    }
}
