using Restia.Common.Identity.Models;
using Restia.Common.Shared.Models;

namespace Restia.Common.Identity.Services.Interfaces
{
    public interface IRoleService
    {
        Task<PagedResult<RoleGetDto>> GetPagedListAsync(SearchParam? searchParam, int page, int pageSize);
        Task<RoleUpdateDto?> GetInfoForUpdateAsync(string id);
        Task<ServiceResult> CreateAsync(RoleCreateDto dto);
        Task<ServiceResult> UpdateAsync(RoleUpdateDto dto);
        Task<ServiceResult> DeleteAsync(string id);
    }
}
