using NotificationEntity = Restia.Common.Notification.Entities.Notification;

namespace Restia.Common.Notification.Service.Interfaces
{
    public interface IBaseNotificationService
    {
        Task CreateAsync(NotificationEntity notification);

        Task<List<NotificationEntity>> GetAsync(string userId, int skip, int take);

        Task<int> GetUnreadCountAsync(string userId);

        Task MarkAsReadAsync(Guid notificationId, string userId);

        Task<int> MarkAllAsReadAsync(string userId);

        Task<NotificationEntity?> FindAsync(Guid notificationId);

        Task<Guid?> CreateAdminAsync(NotificationEntity notification);

        Task TriggerApiAsync(Guid notificationId, string endpoint, string baseUrlKey);
    }
}
