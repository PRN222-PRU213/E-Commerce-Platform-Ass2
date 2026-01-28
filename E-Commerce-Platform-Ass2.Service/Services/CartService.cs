using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ICartItemRepository _cartItemRepository;

        public CartService(ICartRepository cartRepository, ICartItemRepository cartItemRepository)
        {
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
        }

        public async Task AddToCart(Guid userId, Guid productVariantId, int quantity)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                cart = new Cart(Guid.NewGuid(), userId);
                await _cartRepository.CreateAsync(cart);
            }

            var cartItem = await _cartItemRepository.GetByCartAndVariantAsync(cart.Id, productVariantId);
            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
                await _cartItemRepository.UpdateAsync(cartItem);
            }
            else
            {
                cartItem = new CartItem(Guid.NewGuid(), cart.Id, productVariantId, quantity);
                await _cartItemRepository.AddAsync(cartItem);
            }
        }

        public async Task<CartItem> GetCartItemAsync(Guid cartItemId)
        {
            return await _cartItemRepository.GetByIdAsync(cartItemId);
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsByIdsAsync(IEnumerable<Guid> cartItemIds)
        {
            if (cartItemIds == null || !cartItemIds.Any())
            {
                return Enumerable.Empty<CartItem>();
            }

            return await _cartItemRepository.GetItemByIdsAsync(cartItemIds);
        }

        public async Task<decimal> GetCartTotalAsync(Guid userId)
        {
            return await _cartRepository.GetCartTotalAsync(userId);
        }

        public async Task<CartViewModel?> GetCartUserAsync(Guid userId)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(userId);
            if (cart == null)
            {
                return null;
            }

            var cartDTOs = new CartViewModel
            {
                Id = cart.Id,
                Items = cart.CartItems.Select(ci => new CartItemViewModel
                {
                    CartItemId = ci.Id,
                    ProductVariantId = ci.ProductVariantId,
                    ProductId = ci.ProductVariant.ProductId,
                    ProductName = ci.ProductVariant.Product.Name,
                    VariantName = ci.ProductVariant.VariantName,
                    Size = ci.ProductVariant.Size,
                    Color = ci.ProductVariant.Color,
                    ImageUrl = ci.ProductVariant.Product.ImageUrl,
                    Stock = ci.Quantity,
                    Price = ci.ProductVariant.Price
                }).ToList(),
                TotalPrice = cart.CartItems.Sum(ci => ci.Quantity * ci.ProductVariant.Price)
            };

            return cartDTOs;
        }

        public async Task<int> GetTotalItemCountAsync(Guid userId)
        {
            return await _cartRepository.GetTotalItemCountAsync(userId);
        }

        public async Task<bool> RemoveItemAsync(Guid userId, Guid cartItemId)
        {
            var item = await _cartItemRepository.GetByIdAsync(cartItemId);
            if (item == null)
            {
                return false;
            }

            if (item.Cart.UserId != userId)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền xóa món hàng này.");
            }

            await _cartItemRepository.DeleteAsync(item);
            return true;
        }

        public async Task<bool> UpdateQuantityAsync(Guid cartItemId, int quantity)
        {
            var cartItem = await _cartItemRepository.GetByIdAsync(cartItemId);
            if (cartItem == null)
            {
                return false;
            }

            await _cartItemRepository.UpdateQuantityAsync(cartItemId, quantity);
            return true;
        }
    }
}
