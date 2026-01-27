using System;
using System.Collections.Generic;
using System.Linq;

namespace E_Commerce_Platform_Ass2.Wed.Models
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();
        public decimal Shipping { get; set; }
        public decimal Subtotal => Items.Sum(i => i.Total);
        public decimal Total => Subtotal + Shipping;
        public decimal TotalPrice { get; set; } // Map directly from service TotalPrice
    }

    public class CartItemViewModel
    {
        public Guid CartItemId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? Size { get; set; }
        public string? Color { get; set; }
        public decimal Total => Price * Quantity;
        public decimal TotalLinePrice { get; set; } // Map directly from service TotalLinePrice
    }
}
