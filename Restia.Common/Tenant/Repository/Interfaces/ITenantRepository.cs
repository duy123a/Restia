using Restia.Common.Shared.Models;
using Restia.Common.Shared.Repository.Interfaces;
using Restia.Common.Tenant.Models;
using TenantEntity = Restia.Common.Tenant.Entities.Tenant;

namespace Restia.Common.Tenant.Repository.Interfaces
{
    public interface ITenantRepository : IRepository<TenantEntity>
    {
        Task<PagedResult<TenantGetDto>> GetPagedListAsync(
            SearchParam? searchParam,
            int page,
            int pageSize);

        Task<bool> ExistsAnyAsync(string id, CancellationToken cancellationToken = default);

        Task EnsureSchemaCreatedAsync(string schemaId, CancellationToken cancellationToken = default);

        Task MigrateAsync(CancellationToken cancellationToken = default);
    }
}
