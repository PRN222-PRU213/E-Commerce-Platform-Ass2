namespace E_Commerce_Platform_Ass2.Wed.Models.SignalR
{
    public class NotificationMessage
    {
        public string Type { get; set; } = string.Empty; // info | warning | error | success
        public string Message { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public string? Link { get; set; }
        public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    }
}
