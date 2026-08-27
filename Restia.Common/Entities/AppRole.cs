using Microsoft.AspNetCore.Identity;
using Restia.Common.Entities.Interfaces;

namespace Restia.Common.Entities
{
    public class AppRole : IdentityRole, IAuditable
    {
        // Audit
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public string? CreatedBy { get; set; }
        public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
        public string? UpdatedBy { get; set; }
    }
}
