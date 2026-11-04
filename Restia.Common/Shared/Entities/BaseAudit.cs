using Restia.Common.Entities.Interfaces;

namespace Restia.Common.Shared.Entities
{
    public class BaseAudit : IAuditable
    {
        public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public string? CreatedUserId { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public string? UpdatedUserId { get; set; }
    }
}
