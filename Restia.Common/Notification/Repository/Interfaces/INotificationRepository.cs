using Restia.Common.Shared.Repository.Interfaces;
using NotificationEntity = Restia.Common.Notification.Entities.Notification;

namespace Restia.Common.Notification.Repository.Interfaces
{
    public interface INotificationRepository : IRepository<NotificationEntity>
    {
        Task<List<NotificationEntity>> GetByUserAsync(string userId, int skip, int take);

        Task<int> GetUnreadCountAsync(string userId);

        Task<NotificationEntity?> GetByIdAsync(Guid id, string userId);

        Task<int> MarkAllAsReadAsync(string userId);
    }
}
