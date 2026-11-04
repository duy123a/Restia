using Restia.Common.Permission.Constants;
using Restia.Common.Permission.Entities;
using Restia.Common.Permission.Models;
using Restia.Common.Shared.Repository.Interfaces;

namespace Restia.Common.Permission.Repository.Interfaces
{
    public interface IRolePermissionRepository : IRepository<RolePermission>
    {
        Task<List<FormAuthoritySec>> GetRoleAuthorityAsync(IReadOnlyCollection<string> roleNames, string formCode);
        Task<List<PermissionTargetOptionDto>> GetRolePermissionTargetOptionsAsync();
    }
}
