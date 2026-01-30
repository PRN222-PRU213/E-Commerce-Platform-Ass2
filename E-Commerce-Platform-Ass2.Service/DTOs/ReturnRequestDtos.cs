namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    /// <summary>
    /// DTO hiển thị thông tin Return Request
    /// </summary>
    public class ReturnRequestDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        
        public string RequestType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string? ReasonDetail { get; set; }
        public List<string> EvidenceImageUrls { get; set; } = new();
        
        public decimal RequestedAmount { get; set; }
        public decimal? ApprovedAmount { get; set; }
        
        public string Status { get; set; } = string.Empty;
        public string? ShopResponse { get; set; }
        public string? ProcessedByShopName { get; set; }
        public DateTime? ProcessedAt { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Order info
        public decimal OrderTotalAmount { get; set; }
        public string OrderStatus { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public List<ReturnRequestOrderItemDto> OrderItems { get; set; } = new();
    }

    public class ReturnRequestOrderItemDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
    }

    /// <summary>
    /// DTO tạo mới Return Request
    /// </summary>
    public class CreateReturnRequestDto
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string RequestType { get; set; } = "Refund";
        public string Reason { get; set; } = string.Empty;
        public string? ReasonDetail { get; set; }
        public List<string>? EvidenceImageUrls { get; set; }
        public decimal RequestedAmount { get; set; }
    }

    /// <summary>
    /// DTO cho Shop xử lý Return Request
    /// </summary>
    public class ProcessReturnRequestDto
    {
        public Guid RequestId { get; set; }
        public Guid ShopId { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public string? Response { get; set; }
    }
}
