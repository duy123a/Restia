using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using Restia.Common.Identity.Extensions;
using Restia.Common.Localization.Constants;
using Restia.Common.Localization.Resources;
using Restia.Common.Notification.Constants;
using Restia.Common.Notification.Service;
using Restia.Common.Notification.Service.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Restia.Web.Controllers
{
    [Authorize]
    [ApiController]
    [Route("{tenantId}/api/[controller]")]
    public abstract class BaseNotificationController : ControllerBase
    {
        protected readonly IAppNotificationService _notificationService;
        protected readonly IStringLocalizer<SharedResources> _localizer;
        protected readonly IHubContext<NotificationHub> _hubContext;
        protected readonly IConfiguration _config;

        protected BaseNotificationController(
            IAppNotificationService notificationService,
            IStringLocalizer<SharedResources> localizer,
            IHubContext<NotificationHub> hubContext,
            IConfiguration config)
        {
            _notificationService = notificationService;
            _localizer = localizer;
            _hubContext = hubContext;
            _config = config;
        }

        [HttpGet("list")]
        public virtual async Task<IActionResult> GetList(int skip = 0, int take = 10)
        {
            var userId = User.GetUserId();
            var notifications = await _notificationService.GetAsync(userId, skip, take);
            return Ok(notifications);
        }

        [HttpGet("unread-count")]
        public virtual async Task<IActionResult> GetUnreadCount()
        {
            var userId = User.GetUserId();
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(count);
        }

        [HttpPost("mark-as-read/{id}")]
        public virtual async Task<IActionResult> MarkAsRead(Guid id)
        {
            var userId = User.GetUserId();
            await _notificationService.MarkAsReadAsync(id, userId);
            return NoContent();
        }

        [HttpPost("mark-all-as-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.GetUserId();
            var affected = await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { affected });
        }

        [HttpGet("translations")]
        [AllowAnonymous]
        public virtual IActionResult GetTranslations([FromQuery] string? culture = null)
        {
            var requestCulture = culture ??
                Thread.CurrentThread.CurrentCulture.Name
                ?? LocalizationConstants.Default;

            var translations = _localizer.GetAllStrings().ToDictionary(x => x.Name, x => x.Value);

            return Ok(new
            {
                Culture = requestCulture,
                Translations = translations
            });
        }

        protected virtual bool IsValidApiCall(Microsoft.AspNetCore.Http.HttpRequest req)
        {
            var header = req.Headers["X-Notification-Key"].FirstOrDefault();
            var expectedKey = _config["Notification:ApiKey"];

            if (string.IsNullOrEmpty(header) || string.IsNullOrEmpty(expectedKey))
            {
                return false;
            }

            // Protect timing attack
            return CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(header),
                Encoding.UTF8.GetBytes(expectedKey));
        }

        [HttpPost("trigger")]
        [AllowAnonymous]
        public virtual async Task<IActionResult> Trigger([FromBody] Guid notificationId)
        {
            if (!IsValidApiCall(Request)) return Unauthorized();

            var notif = await _notificationService.FindAsync(notificationId);
            if (notif == null) return NotFound();

            var dto = new
            {
                id = notif.Id,
                userId = notif.UserId,
                titleKey = notif.TitleKey,
                titleParams = notif.TitleParams,
                messageKey = notif.MessageKey,
                messageParams = notif.MessageParams,
                url = notif.Url,
                createdAt = notif.CreatedAt
            };

            if (!string.IsNullOrEmpty(notif.UserId))
            {
                await _hubContext.Clients.User(notif.UserId)
                    .SendAsync(NotificationConstants.NewNotification, dto);
            }
            else
            {
                throw new ArgumentException("User not found.");
            }

            return Ok();
        }

        protected virtual async Task<IActionResult> TriggerGroup(Guid notificationId, List<string> groupNameList)
        {
            if (!IsValidApiCall(Request)) return Unauthorized();

            var notif = await _notificationService.FindAsync(notificationId);
            if (notif == null) return NotFound();

            var dto = new
            {
                id = notif.Id,
                titleKey = notif.TitleKey,
                titleParams = notif.TitleParams,
                messageKey = notif.MessageKey,
                messageParams = notif.MessageParams,
                url = notif.Url,
                createdAt = notif.CreatedAt
            };

            foreach (var groupName in groupNameList)
            {
                await _hubContext.Clients.Group(groupName).SendAsync(NotificationConstants.NewNotification, dto);
            }

            return Ok();
        }
    }
}
