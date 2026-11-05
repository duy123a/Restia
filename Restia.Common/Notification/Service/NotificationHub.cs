using Microsoft.AspNetCore.SignalR;

namespace Restia.Common.Notification.Service
{
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var user = Context.User;
            if (user != null)
            {
                var roles = user.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role).Select(c => c.Value);
                foreach (var role in roles)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, "Role_" + role);
                }
            }

            await base.OnConnectedAsync();
        }
    }
}
