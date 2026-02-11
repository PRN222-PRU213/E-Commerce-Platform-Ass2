using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;

        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<OrderDto>> GetOrderHistoryAsync(Guid userId)
        {
            var orders = await _orderRepository.GetByUserIdAsync(userId);

            if (!orders.Any())
            {
                return new List<OrderDto>();
            }

            var orderDtos = orders.Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                TotalAmount = o.TotalAmount,
                ShippingAddress = o.ShippingAddress,
                Status = o.Status,
                CreatedAt = o.CreatedAt
            }).ToList();

            return orderDtos;
        }

        public async Task<OrderDetailDto?> GetOrderItemAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);

            if (order == null)
            {
                throw new Exception("Not found.");
            }

            var orderDto = new OrderDetailDto
            {
                Id = order.Id,
                UserId = order.UserId,
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                Items = order.OrderItems.Select(oi => new OrderItemDto
                {
                    Id = oi.Id,
                    ProductVariantId = oi.ProductVariantId,
                    ProductName = oi.ProductName,
                    Size = oi.ProductVariant.Size,
                    Color = oi.ProductVariant.Color,
                    Quantity = oi.Quantity,
                    ImageUrl = oi.ProductVariant.ImageUrl,
                    Price = oi.Price
                }).ToList()
            };

            return orderDto;
        }

        public async Task<IEnumerable<Guid>> GetShopIdsByOrderAsync(Guid orderId)
        {
            var order = await _orderRepository.GetByIdWithDetailsAsync(orderId);
            if (order == null) return Enumerable.Empty<Guid>();

            return order.OrderItems
                .Where(oi => oi.ProductVariant?.Product != null)
                .Select(oi => oi.ProductVariant.Product.ShopId)
                .Distinct()
                .ToList();
        }
    }
}
