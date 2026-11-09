using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using Restia.Common.Identity.Enums;
using Restia.Common.Localization.Resources;
using Restia.Common.Notification.Constants;
using Restia.Common.Notification.Service;
using Restia.Common.Notification.Service.Interfaces;
using Restia.Common.Tenant.Services.Interfaces;

namespace Restia.Web.Controllers
{
    public class NotificationController : BaseNotificationController
    {
        private readonly ITenantContext _tenantContext;

        public NotificationController(
            IAppNotificationService notificationService,
            IStringLocalizer<SharedResources> localizer,
            IHubContext<NotificationHub> hubContext,
            IConfiguration config,
            ITenantContext tenantContext) : base(notificationService, localizer, hubContext, config)
        {
            _tenantContext = tenantContext;
        }

        [HttpPost("trigger-all-admin")]
        [AllowAnonymous]
        public async Task<IActionResult> TriggerAllAdmin([FromBody] Guid notificationId)
        {
            var schemaId = _tenantContext.SchemaId;
            return await TriggerGroup(notificationId, [
                NotificationConstants.GetTenantRoleGroup(schemaId, AppRoles.Admin.ToString()),
                NotificationConstants.GetTenantRoleGroup(schemaId, AppRoles.SuperAdmin.ToString())
            ]);
        }
    }
}
