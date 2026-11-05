using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Restia.Common.Identity.Entities;
using Restia.Common.Notification.Service.Interfaces;
using Restia.Common.Shared.UnitOfWorks.Interfaces;

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

        public async Task SelfTriggerAsync(Guid notificationId)
        {
            await TriggerApiAsync(notificationId, "trigger", "AdminSite:BaseUrl");
        }

        public async Task SelfTriggerAdminAsync(Guid notificationId)
        {
            await TriggerApiAsync(notificationId, "trigger-all-admin", "AdminSite:BaseUrl");
        }
    }
}
