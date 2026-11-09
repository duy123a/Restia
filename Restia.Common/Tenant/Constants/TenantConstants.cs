using System.Text.RegularExpressions;

namespace Restia.Common.Tenant.Constants
{
    public static partial class TenantConstants
    {
        public const string DefaultTenantId = "public";
        public const string RouteKey = "tenantId";
        public const string ActiveTenantsCacheKey = "Restia_ActiveTenants";

        [GeneratedRegex("^[a-z][a-z0-9_]{1,62}$", RegexOptions.Compiled)]
        private static partial Regex SchemaNameRegex();

        public static bool IsValidSchemaName(string? schemaId)
        {
            if (string.IsNullOrWhiteSpace(schemaId))
            {
                return false;
            }

            // Prevent reserved schema of postgres
            if (schemaId.StartsWith("pg_", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(schemaId, "information_schema", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return SchemaNameRegex().IsMatch(schemaId);
        }
    }
}
