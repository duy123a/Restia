using Restia.Common.Tenant.Constants;
using Restia.Common.Tenant.Services.Interfaces;

namespace Restia.Common.Tenant.Services
{
    public class TenantContext : ITenantContext
    {
        public string SchemaId { get; private set; } = TenantConstants.DefaultTenantId;

        public bool IsPublic => string.Equals(
            SchemaId,
            TenantConstants.DefaultTenantId,
            StringComparison.OrdinalIgnoreCase);

        public void SetTenant(string schemaId)
        {
            if (!TenantConstants.IsValidSchemaName(schemaId))
            {
                throw new ArgumentException($"Invalid schema name: {schemaId}", nameof(schemaId));
            }

            SchemaId = schemaId.ToLowerInvariant();
        }
    }
}
