using E_Commerce_Platform_Ass2.Data.Database.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass2.Data.Repositories.Interfaces
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetByUserIdAsync(Guid userId, int count = 20);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task<Notification> AddAsync(Notification notification);
        Task MarkAllAsReadAsync(Guid userId);
        Task<Notification?> GetByIdAsync(Guid id);
        Task UpdateAsync(Notification notification);
    }
}
