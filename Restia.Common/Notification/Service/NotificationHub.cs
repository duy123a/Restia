using Microsoft.AspNetCore.SignalR;
using Restia.Common.Identity.Extensions;
using Restia.Common.Notification.Constants;

namespace Restia.Common.Notification.Service
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            if (user != null)
            {
                var schemaId = user.GetSchemaId();
                var roles = user.GetRoles();
                foreach (var role in roles)
                {
                    await Groups.AddToGroupAsync(
                        Context.ConnectionId,
                        NotificationConstants.GetTenantRoleGroup(schemaId, role));
                }
            }

            await base.OnConnectedAsync();
        }
    }
}
