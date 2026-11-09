using Restia.Common.Shared.Entities;

namespace Restia.Common.Tenant.Entities
{
    public class Tenant : BaseEntity
    {
        public string Id { get; set; } = string.Empty;

        public string DisplayName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}
