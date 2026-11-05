using Microsoft.EntityFrameworkCore;
using Restia.Common.Data.Context;
using Restia.Common.Notification.Repository.Interfaces;
using Restia.Common.Shared.Repository;
using NotificationEntity = Restia.Common.Notification.Entities.Notification;

namespace Restia.Common.Notification.Repository
{
    public class NotificationRepository : BaseRepository<NotificationEntity>, INotificationRepository
    {
        public NotificationRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<NotificationEntity>> GetByUserAsync(string userId, int skip, int take)
        {
            return await Context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await Context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task<NotificationEntity?> GetByIdAsync(Guid id, string userId)
        {
            return await Context.Notifications
                .Where(n => n.Id == id && n.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<int> MarkAllAsReadAsync(string userId)
        {
            return await Context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        }
    }
}
