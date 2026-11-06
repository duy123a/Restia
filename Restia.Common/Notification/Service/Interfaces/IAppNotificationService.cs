namespace Restia.Common.Notification.Service.Interfaces
{
    public interface IAppNotificationService : IBaseNotificationService
    {
        Task TriggerAsync(Guid notificationId);

        Task TriggerAdminAsync(Guid notificationId);

        Task NotifyDemoAsync(string userId);

        Task NotifyAdminDemoAsync();
    }
}
