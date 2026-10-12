using Restia.Common.Entities.Interfaces;

namespace Restia.Common.Shared.Entities
{
    public abstract class BaseEntity : IAuditable, ISoftDeletable
    {
        public uint RV { get; set; }

        public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public string? CreatedUserId { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        public string? UpdatedUserId { get; set; }

        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public string? DeletedUserId { get; set; }
    }
}
