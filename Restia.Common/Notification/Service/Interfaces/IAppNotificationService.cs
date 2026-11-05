namespace Restia.Common.Notification.Service.Interfaces
{
    public interface IAppNotificationService : IBaseNotificationService
    {
        Task SelfTriggerAsync(Guid notificationId);

        Task SelfTriggerAdminAsync(Guid notificationId);
    }
}
