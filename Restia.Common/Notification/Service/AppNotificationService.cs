using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Restia.Common.Identity.Entities;
using Restia.Common.Notification.Service.Interfaces;
using Restia.Common.Shared.UnitOfWorks.Interfaces;
using NotificationEntity = Restia.Common.Notification.Entities.Notification;

namespace Restia.Common.Notification.Service
{
    public class AppNotificationService : BaseNotificationService, IAppNotificationService
    {
        public AppNotificationService(
            IAdminUnitOfWork notificationUnitOfWork,
            IHttpClientFactory httpFactory,
            IConfiguration config,
            ILogger<AppNotificationService> logger,
            UserManager<AppUser> userManager)
            : base(notificationUnitOfWork, httpFactory, config, logger, userManager)
        {
        }

        public async Task TriggerAsync(Guid notificationId)
        {
            await TriggerApiAsync(notificationId, "trigger", "AdminSite:BaseUrl");
        }

        public async Task TriggerAdminAsync(Guid notificationId)
        {
            await TriggerApiAsync(notificationId, "trigger-all-admin", "AdminSite:BaseUrl");
        }

        public async Task NotifyDemoAsync(string userId)
        {
            try
            {
                var appUser = await _userManager.FindByIdAsync(userId);
                if (appUser == null)
                {
                    _logger.LogWarning("Cannot send notification. User {UserId} not found", userId);
                    return;
                }

                var notification = new NotificationEntity
                {
                    UserId = userId,
                    TitleKey = "AppNotice_DemoNoticeTitle",
                    MessageKey = "AppNotice_DemoNoticeMessage",
                    Url = _config["AdminSite:BaseUrl"]
                };
                await CreateAsync(notification);
                await TriggerAsync(notification.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send user status notification for UserId {UserId}", userId);
            }
        }

        public async Task NotifyAdminDemoAsync()
        {
            try
            {
                var notification = new NotificationEntity
                {
                    TitleKey = "AppNotice_DemoAdminNoticeTitle",
                    MessageKey = "AppNotice_DemoAdminNoticeMessage",
                    Url = _config["AdminSite:BaseUrl"]
                };
                var notifId = await CreateAdminAsync(notification);
                if (notifId.HasValue)
                {
                    await TriggerAdminAsync(notifId.Value);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send admin demo notification");
            }
        }
    }
}
