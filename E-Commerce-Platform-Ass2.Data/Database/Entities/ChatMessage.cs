using System;

namespace E_Commerce_Platform_Ass2.Data.Database.Entities
{
    public class ChatMessage
    {
        public Guid Id { get; set; }
        
        public Guid ChatSessionId { get; set; }
        public ChatSession ChatSession { get; set; } = null!;
        
        public Guid? SenderId { get; set; }
        public string SenderRole { get; set; } = string.Empty; // "Customer", "Shop", "AI"
        
        public string Content { get; set; } = string.Empty;
        
        public Guid? ProductId { get; set; }
        public Product? Product { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
