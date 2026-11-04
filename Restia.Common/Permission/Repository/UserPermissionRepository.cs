using Microsoft.EntityFrameworkCore;
using Restia.Common.Data.Context;
using Restia.Common.Permission.Entities;
using Restia.Common.Permission.Models;
using Restia.Common.Permission.Repository.Interfaces;
using Restia.Common.Shared.Repository;

namespace Restia.Common.Permission.Repository
{
    public class UserPermissionRepository : BaseRepository<UserPermission>, IUserPermissionRepository
    {
        public UserPermissionRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<PermissionTargetOptionDto>> GetUserPermissionTargetOptionsAsync()
        {
            return await Context.Users
                .OrderBy(u => u.UserName)
                .Select(u => new PermissionTargetOptionDto(
                    u.Id,
                    u.UserName ?? u.Email ?? string.Empty))
                .ToListAsync();
        }
    }
}
