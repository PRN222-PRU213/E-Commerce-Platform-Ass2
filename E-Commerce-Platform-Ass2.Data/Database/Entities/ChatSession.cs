using System;
using System.Collections.Generic;

namespace E_Commerce_Platform_Ass2.Data.Database.Entities
{
    public class ChatSession
    {
        public Guid Id { get; set; }
        
        public Guid CustomerId { get; set; }
        public User Customer { get; set; } = null!;
        
        public Guid ShopId { get; set; }
        public Shop Shop { get; set; } = null!;
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public ICollection<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
