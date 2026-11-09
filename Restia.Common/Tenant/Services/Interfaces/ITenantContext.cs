namespace Restia.Common.Tenant.Services.Interfaces
{
    public interface ITenantContext
    {
        string SchemaId { get; }

        bool IsPublic { get; }

        void SetTenant(string schemaId);
    }
}
