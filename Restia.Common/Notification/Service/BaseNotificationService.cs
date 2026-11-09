using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Restia.Common.Identity.Entities;
using Restia.Common.Identity.Enums;
using Restia.Common.Notification.Service.Interfaces;
using Restia.Common.Shared.UnitOfWorks.Interfaces;
using Restia.Common.Tenant.Services.Interfaces;
using System.Net.Http.Json;
using NotificationEntity = Restia.Common.Notification.Entities.Notification;

namespace Restia.Common.Notification.Service
{
    public abstract class BaseNotificationService : IBaseNotificationService
    {
        protected readonly IAdminUnitOfWork _uow;
        protected readonly IHttpClientFactory _httpFactory;
        protected readonly IConfiguration _config;
        protected readonly ILogger _logger;
        protected readonly UserManager<AppUser> _userManager;
        protected readonly ITenantContext _tenantContext;

        protected BaseNotificationService(
            IAdminUnitOfWork uow,
            IHttpClientFactory httpFactory,
            IConfiguration config,
            ILogger logger,
            UserManager<AppUser> userManager,
            ITenantContext tenantContext)
        {
            _uow = uow;
            _httpFactory = httpFactory;
            _config = config;
            _logger = logger;
            _userManager = userManager;
            _tenantContext = tenantContext;
        }

        public async Task CreateAsync(NotificationEntity notification)
        {
            if (notification == null) return;

            await _uow.NotificationRepository.AddAsync(notification);
            await _uow.SaveChangesAsync();
        }

        public async Task<List<NotificationEntity>> GetAsync(string userId, int skip, int take)
        {
            return await _uow.NotificationRepository.GetByUserAsync(userId, skip, take);
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _uow.NotificationRepository.GetUnreadCountAsync(userId);
        }

        public async Task MarkAsReadAsync(Guid notificationId, string userId)
        {
            var notification = await _uow.NotificationRepository.GetByIdAsync(notificationId, userId);

            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                _uow.NotificationRepository.Update(notification);
                await _uow.SaveChangesAsync();
            }
        }

        public async Task<int> MarkAllAsReadAsync(string userId)
        {
            var affected = await _uow.NotificationRepository.MarkAllAsReadAsync(userId);
            if (affected > 0)
            {
                await _uow.SaveChangesAsync();
            }

            return affected;
        }

        public async Task<NotificationEntity?> FindAsync(Guid notificationId)
        {
            return await _uow.NotificationRepository.GetByIdAsync(notificationId);
        }

        public async Task<Guid?> CreateAdminAsync(NotificationEntity notification)
        {
            if (notification == null) return null;

            var adminUsers = await _userManager.GetUsersInRoleAsync(AppRoles.Admin.ToString());
            var superAdminUsers = await _userManager.GetUsersInRoleAsync(AppRoles.SuperAdmin.ToString());
            var users = adminUsers
                .Concat(superAdminUsers)
                .DistinctBy(u => u.Id)
                .ToList();

            NotificationEntity? firstNotif = null;

            foreach (var user in users)
            {
                var notif = new NotificationEntity
                {
                    UserId = user.Id,
                    TitleKey = notification.TitleKey,
                    TitleParams = notification.TitleParams,
                    MessageKey = notification.MessageKey,
                    MessageParams = notification.MessageParams,
                    Url = notification.Url,
                    CreatedAt = DateTimeOffset.UtcNow
                };

                await _uow.NotificationRepository.AddAsync(notif);

                // use first id as a representation
                firstNotif ??= notif;
            }

            await _uow.SaveChangesAsync();

            return firstNotif?.Id;
        }

        public async Task TriggerApiAsync(Guid notificationId, string endpoint, string baseUrlKey)
        {
            var client = _httpFactory.CreateClient();
            var apiKey = _config["Notification:ApiKey"];
            if (!string.IsNullOrEmpty(apiKey))
            {
                client.DefaultRequestHeaders.Add("X-Notification-Key", apiKey);
            }

            try
            {
                var baseUrl = _config[baseUrlKey]?.TrimEnd('/');
                var schemaId = _tenantContext.SchemaId;
                var res = await client.PostAsJsonAsync($"{baseUrl}/{schemaId}/api/notification/{endpoint}", notificationId);
                if (!res.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Trigger to {BaseUrlKey} failed: {Status}", baseUrlKey, res.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling {BaseUrlKey} trigger API for endpoint {Endpoint}", baseUrlKey, endpoint);
            }
        }
    }
}
