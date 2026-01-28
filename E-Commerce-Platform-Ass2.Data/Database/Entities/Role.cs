using System;
using System.Collections.Generic;

namespace E_Commerce_Platform_Ass1.Data.Database.Entities
{
    public class Role
    {
        public Guid RoleId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        // Navigation property
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
