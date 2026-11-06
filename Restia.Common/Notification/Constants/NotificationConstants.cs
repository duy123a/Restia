using Restia.Common.Identity.Enums;

namespace Restia.Common.Notification.Constants
{
    public class NotificationConstants
    {
        public const string NewNotification = "NewNotification";

        public static readonly string AdminGroup = $"Role_{AppRoles.Admin}";

        public static readonly string SuperAdminGroup = $"Role_{AppRoles.SuperAdmin}";
    }
}
