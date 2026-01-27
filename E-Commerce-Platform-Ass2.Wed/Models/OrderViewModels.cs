namespace E_Commerce_Platform_Ass2.Wed.Models
{
    public class OrderHistoryViewModel
    {
        public Guid OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public bool IsRefunded { get; set; }
    }

    public class OrderDetailViewModel
    {
        public Guid OrderId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ShippingAddress { get; set; } = "";
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "";

        public List<OrderItemViewModel> Items { get; set; } = new();
    }

    public class OrderItemViewModel
    {
        public string ProductName { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public decimal Price { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public int Quantity { get; set; }
        public decimal SubTotal => Price * Quantity;
    }
}
