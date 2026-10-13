using Microsoft.EntityFrameworkCore;
using Restia.Common.Data.Context;
using Restia.Common.Identity.Models;
using Restia.Common.Identity.Repository.Interfaces;
using Restia.Common.Permission.Entities;
using Restia.Common.Shared.Entities;
using Restia.Common.Shared.Models;
using Restia.Common.Shared.Repository;

namespace Restia.Common.Identity.Repository
{
    public class RoleRepository : BaseRepository<AppRole>, IRoleRepository
    {
        public RoleRepository(AppDbContext context)
            : base(context)
        {
        }

        public async Task<IReadOnlyList<RoleOptionDto>>
            GetAllRoleOptionsAsync()
        {
            return await Context.Roles
                .OrderBy(r => r.Name)
                .Select(r => new RoleOptionDto(r.Id, r.Name!))
                .ToListAsync();
        }

        public async Task<PagedResult<RoleGetDto>> GetPagedListAsync(
            string? search,
            int page,
            int pageSize)
        {
            var query = Context.Roles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();

                query = query.Where(r =>
                    r.Name != null && r.Name.Contains(term));
            }

            var totalCount = await query.CountAsync();

            var roles = await query
                .OrderBy(r => r.Name)
                .ThenBy(r => r.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var roleIds = roles.Select(r => r.Id).ToArray();

            var permissionCounts = await Context.RolePermissions
                .Where(rp => roleIds.Contains(rp.RoleId))
                .GroupBy(rp => rp.RoleId)
                .Select(g => new
                {
                    RoleId = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.RoleId, x => x.Count);

            var userCounts = await Context.UserRoles
                .Where(ur => roleIds.Contains(ur.RoleId))
                .GroupBy(ur => ur.RoleId)
                .Select(g => new
                {
                    RoleId = g.Key,
                    Count = g.Count()
                })
                .ToDictionaryAsync(x => x.RoleId, x => x.Count);

            var items = roles.Select(r => new RoleGetDto(
                Id: r.Id,
                Name: r.Name ?? string.Empty,
                PermissionCount: permissionCounts.GetValueOrDefault(r.Id),
                UserCount: userCounts.GetValueOrDefault(r.Id)
            )).ToList();

            return new PagedResult<RoleGetDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<IReadOnlyList<RolePermissionDto>>
            GetPermissionsByRoleIdAsync(string roleId)
        {
            return await Context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .OrderBy(rp => rp.FormCode)
                .Select(rp => new RolePermissionDto(
                    rp.FormCode,
                    rp.FormAuthoritySec))
                .ToListAsync();
        }

        public async Task ReplacePermissionsAsync(
            string roleId,
            IEnumerable<RolePermissionDto> permissions)
        {
            var existing = await Context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            Context.RolePermissions.RemoveRange(existing);

            var newPermissions = permissions
                .GroupBy(p => p.FormCode, StringComparer.OrdinalIgnoreCase)
                .Select(g => g.Last())
                .Select(p => new RolePermission
                {
                    RoleId = roleId,
                    FormCode = p.FormCode,
                    FormAuthoritySec = p.FormAuthoritySec
                })
                .ToList();

            await Context.RolePermissions.AddRangeAsync(newPermissions);
            await Context.SaveChangesAsync();
        }
    }
}