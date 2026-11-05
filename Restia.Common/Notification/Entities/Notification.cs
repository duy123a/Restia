using Restia.Common.Identity.Entities;

namespace Restia.Common.Notification.Entities
{
    public class Notification
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string UserId { get; set; } = null!;
        public AppUser User { get; set; } = null!;

        public string? SenderId { get; set; }
        public AppUser? Sender { get; set; }

        public string TitleKey { get; set; } = null!;
        public string? TitleParams { get; set; }

        public string MessageKey { get; set; } = null!;
        public string? MessageParams { get; set; }

        public string? Url { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
