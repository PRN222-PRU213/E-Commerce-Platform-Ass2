namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    public class CreatePreOrderRequest
    {
        public Guid ProductId { get; set; }
        public Guid VariantId { get; set; }
        public int Quantity { get; set; }
        public decimal? DepositPercent { get; set; }
        public DateTime? ExpectedAvailableDate { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
    }

    public class PayPreOrderDepositRequest
    {
        public Guid PreOrderId { get; set; }
        public string PaymentMethod { get; set; } = "WALLET";
    }

    public class FinalizePreOrderPaymentRequest
    {
        public Guid PreOrderId { get; set; }
        public string PaymentMethod { get; set; } = "WALLET";
    }

    public class MarkPreOrderReadyRequest
    {
        public Guid PreOrderId { get; set; }
        public int FinalPaymentDeadlineHours { get; set; } = 24;
    }

    public class PreOrderStatusDto
    {
        public Guid PreOrderId { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public Guid ShopId { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal DepositAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? FinalPaymentDeadline { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class PreOrderSummaryDto
    {
        public Guid PreOrderId { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public Guid ShopId { get; set; }
        public string PreOrderStatus { get; set; } = string.Empty;
        public string OrderStatus { get; set; } = string.Empty;
        public decimal DepositAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime ExpectedAvailableDate { get; set; }
        public DateTime? FinalPaymentDueAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
