using System;

namespace E_Commerce_Platform_Ass2.Wed.Models.SignalR
{
    public class TicketNotificationMessage
    {
        public Guid TicketId { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string? ShopId { get; set; }
    }

    public class TicketReplyNotificationMessage
    {
        public Guid TicketId { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public Guid ReplyId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderRole { get; set; } = string.Empty;
        public string Preview { get; set; } = string.Empty;
    }
}
