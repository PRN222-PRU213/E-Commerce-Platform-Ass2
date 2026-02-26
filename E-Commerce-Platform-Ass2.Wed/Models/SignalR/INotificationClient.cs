using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass2.Wed.Models.SignalR
{
    /// <summary>
    /// Typed client giúp tránh sai tên sự kiện ở client.
    /// </summary>
    public interface INotificationClient
    {
        Task ProductChanged(ProductChangedMessage message);
        Task NotificationReceived(NotificationMessage message);
        Task ReviewApproved(E_Commerce_Platform_Ass2.Service.DTOs.ReviewDto review);
        Task ReviewSubmitted(E_Commerce_Platform_Ass2.Service.DTOs.ReviewDto review);
    }
}
