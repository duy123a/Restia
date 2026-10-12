using Microsoft.AspNetCore.Identity;
using Restia.Common.Entities.Interfaces;

namespace Restia.Common.Shared.Entities
{
    public class AppUser : IdentityUser, IAuditable
    {
        public string DisplayName { get; set; } = string.Empty;
        public string ProfileImg { get; set; } = string.Empty;

        // Audit
        public DateTimeOffset? CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public string? CreatedUserId { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
        public string? UpdatedUserId { get; set; }
    }
}
