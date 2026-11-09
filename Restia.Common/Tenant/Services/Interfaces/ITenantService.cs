using Restia.Common.Shared.Models;
using Restia.Common.Tenant.Models;

namespace Restia.Common.Tenant.Services.Interfaces
{
    public interface ITenantService
    {
        Task<PagedResult<TenantGetDto>> GetPagedListAsync(
            SearchParam? searchParam,
            int page,
            int pageSize);

        Task<TenantUpdateDto?> GetInfoForUpdateAsync(string id);

        Task<ServiceResult> CreateAsync(TenantCreateDto dto);

        Task<ServiceResult> UpdateAsync(TenantUpdateDto dto);

        Task<ServiceResult> DeleteAsync(string id);

        Task<bool> IsValidActiveTenantAsync(string? schemaId);

        Task InitializeAllTenantsAsync(CancellationToken cancellationToken = default);
    }
}
