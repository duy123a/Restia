using Microsoft.EntityFrameworkCore;
using Restia.Common.Data.Context;
using Restia.Common.Permission.Constants;
using Restia.Common.Permission.Entities;
using Restia.Common.Permission.Models;
using Restia.Common.Permission.Repository.Interfaces;
using Restia.Common.Shared.Repository;

namespace Restia.Common.Permission.Repository
{
    public class RolePermissionRepository : BaseRepository<RolePermission>, IRolePermissionRepository
    {
        public RolePermissionRepository(AppDbContext context) : base(context)
        {
        }

        public Task<List<FormAuthoritySec>> GetRoleAuthorityAsync(IReadOnlyCollection<string> roleNames, string formCode)
        {
            return Context.RolePermissions
                .AsNoTracking()
                .Join(
                    Context.Roles.AsNoTracking(),
                    rp => rp.RoleId,
                    r => r.Id,
                    (rp, r) => new { rp, r }
                )
                .Where(x =>
                    x.r.Name != null &&
                    roleNames.Contains(x.r.Name) &&
                    x.rp.FormCode == formCode
                )
                .Select(x => x.rp.FormAuthoritySec)
                .ToListAsync();
        }

        public async Task<List<PermissionTargetOptionDto>> GetRolePermissionTargetOptionsAsync()
        {
            return await Context.Roles
                .OrderBy(r => r.Name)
                .Select(r => new PermissionTargetOptionDto(
                    r.Id,
                    r.Name ?? string.Empty))
                .ToListAsync();
        }
    }
}
