namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    public class CartViewModel
    {
        public Guid Id { get; set; }

        public List<CartItemViewModel> Items { get; set; } = new List<CartItemViewModel>();

        public decimal TotalPrice { get; set; }
    }
}
