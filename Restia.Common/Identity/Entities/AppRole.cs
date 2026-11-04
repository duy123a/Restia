using Microsoft.AspNetCore.Identity;
using Restia.Common.Entities.Interfaces;

namespace Restia.Common.Identity.Entities
{
    public class AppRole : IdentityRole, IAuditable
    {
        // Audit
        public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public string? CreatedUserId { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public string? UpdatedUserId { get; set; }
    }
}
