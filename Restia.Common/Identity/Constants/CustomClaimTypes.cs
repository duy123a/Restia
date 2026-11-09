namespace Restia.Common.Identity.Constants
{
    public static class CustomClaimTypes
    {
        private const string SCHEMA = "$/identity/claims";

        public const string DisplayName = $"{SCHEMA}/display_name";
        public const string SchemaId = $"{SCHEMA}/schema_id";
    }
}
