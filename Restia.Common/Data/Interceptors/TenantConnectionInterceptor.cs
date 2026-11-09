using Microsoft.EntityFrameworkCore.Diagnostics;
using Restia.Common.Tenant.Constants;
using Restia.Common.Tenant.Services.Interfaces;
using System.Data.Common;

namespace Restia.Common.Data.Interceptors
{
    public class TenantConnectionInterceptor : DbConnectionInterceptor
    {
        private readonly ITenantContext _tenantContext;

        public TenantConnectionInterceptor(ITenantContext tenantContext)
        {
            _tenantContext = tenantContext;
        }

        public override void ConnectionOpened(
            DbConnection connection,
            ConnectionEndEventData eventData)
        {
            var schemaId = _tenantContext.SchemaId;
            if (!TenantConstants.IsValidSchemaName(schemaId))
            {
                throw new InvalidOperationException($"Invalid tenant schema: {schemaId}");
            }

            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"SET search_path TO \"{schemaId}\";";
            cmd.ExecuteNonQuery();
        }

        public override async Task ConnectionOpenedAsync(
            DbConnection connection,
            ConnectionEndEventData eventData,
            CancellationToken cancellationToken = default)
        {
            var schemaId = _tenantContext.SchemaId;
            if (!TenantConstants.IsValidSchemaName(schemaId))
            {
                throw new InvalidOperationException($"Invalid tenant schema: {schemaId}");
            }

            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"SET search_path TO \"{schemaId}\";";
            await cmd.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
