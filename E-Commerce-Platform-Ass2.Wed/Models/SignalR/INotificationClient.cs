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
    }
}
