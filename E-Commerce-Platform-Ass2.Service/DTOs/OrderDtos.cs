namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    /// <summary>
    /// DTO hiển thị đơn hàng trong danh sách
    /// </summary>
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ItemCount { get; set; }

        // Thông tin vận chuyển
        public string? Carrier { get; set; }
        public string? TrackingCode { get; set; }
        public string? ShipmentStatus { get; set; }
    }

    /// <summary>
    /// DTO chi tiết đơn hàng
    /// </summary>
    public class OrderDetailDto : OrderDto
    {
        public List<OrderItemDto> Items { get; set; } = new();
        public PaymentDto? Payment { get; set; }
        public ShipmentDto? Shipment { get; set; }
    }

    /// <summary>
    /// DTO item trong đơn hàng
    /// </summary>
    public class OrderItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductVariantId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? VariantName { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice => Price * Quantity;
    }

    /// <summary>
    /// DTO thanh toán
    /// </summary>
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public string Method { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? TransactionCode { get; set; }
        public DateTime PaidAt { get; set; }
    }

    /// <summary>
    /// DTO vận chuyển
    /// </summary>
    public class ShipmentDto
    {
        public Guid Id { get; set; }
        public string Carrier { get; set; } = string.Empty;
        public string TrackingCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO tạo shipment khi gửi hàng
    /// </summary>
    public class CreateShipmentDto
    {
        public string Carrier { get; set; } = string.Empty;
        public string TrackingCode { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO cập nhật trạng thái vận chuyển
    /// </summary>
    public class UpdateShipmentDto
    {
        public string Carrier { get; set; } = string.Empty;
        public string TrackingCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
