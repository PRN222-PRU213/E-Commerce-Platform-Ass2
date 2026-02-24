using E_Commerce_Platform_Ass2.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(Guid userId, int count = 20);
        Task<int> GetUnreadCountAsync(Guid userId);
        Task<NotificationDto> CreateNotificationAsync(Guid userId, string type, string message, string? link = null);
        Task MarkAllAsReadAsync(Guid userId);
        Task MarkAsReadAsync(Guid id);
        Task DeleteAsync(Guid userId, Guid notificationId);
        Task DeleteAllAsync(Guid userId);
    }
}
