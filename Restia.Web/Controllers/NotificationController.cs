using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using Restia.Common.Identity.Entities;
using Restia.Common.Localization.Resources;
using Restia.Common.Notification.Constants;
using Restia.Common.Notification.Service;
using Restia.Common.Notification.Service.Interfaces;

namespace Restia.Web.Controllers
{
    public class NotificationController : BaseNotificationController
    {
        public NotificationController(
            IAppNotificationService notificationService,
            UserManager<AppUser> userManager,
            IStringLocalizer<SharedResources> localizer,
            IHubContext<NotificationHub> hubContext,
            IConfiguration config) : base(notificationService, userManager, localizer, hubContext, config)
        {
        }

        [HttpPost("trigger-all-admin")]
        [AllowAnonymous]
        public async Task<IActionResult> TriggerAllAdmin([FromBody] Guid notificationId)
        {
            return await TriggerGroup(notificationId, [NotificationConstants.AdminGroup, NotificationConstants.SuperAdminGroup]);
        }
    }
}
