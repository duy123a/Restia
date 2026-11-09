using Restia.Common.Identity.Enums;
using Restia.Common.Tenant.Constants;

namespace Restia.Common.Notification.Constants
{
    public class NotificationConstants
    {
        public const string NewNotification = "NewNotification";

        public static readonly string AdminGroup = $"Role_{AppRoles.Admin}";

        public static readonly string SuperAdminGroup = $"Role_{AppRoles.SuperAdmin}";

        public static string GetTenantRoleGroup(string? schemaId, string role)
        {
            var tenant = string.IsNullOrWhiteSpace(schemaId) ? TenantConstants.DefaultTenantId : schemaId.ToLowerInvariant();
            return $"Tenant_{tenant}_Role_{role}";
        }
    }
}
