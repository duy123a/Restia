using Restia.Common.Identity.Models;
using Restia.Common.Shared.Entities;
using Restia.Common.Shared.Models;
using Restia.Common.Shared.Repository.Interfaces;

namespace Restia.Common.Identity.Repository.Interfaces
{
    public interface IRoleRepository : IRepository<AppRole>
    {
        Task<IReadOnlyList<RoleOptionDto>> GetAllRoleOptionsAsync();

        Task<PagedResult<RoleGetDto>> GetPagedListAsync(
            SearchParam? searchParam,
            int page,
            int pageSize);

        Task<IReadOnlyList<RolePermissionDto>> GetPermissionsByRoleIdAsync(string roleId);

        Task ReplacePermissionsAsync(
            string roleId,
            IEnumerable<RolePermissionDto> permissions);
    }
}
