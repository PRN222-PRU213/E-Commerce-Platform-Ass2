namespace E_Commerce_Platform_Ass2.Wed.Models
{
    public class TopUpCallbackViewModel
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal NewBalance { get; set; }
    }
}
