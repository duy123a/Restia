using Microsoft.EntityFrameworkCore;
using Restia.Common.Data.Context;
using Restia.Common.Identity.Models;
using Restia.Common.Identity.Repository.Interfaces;
using Restia.Common.Shared.Constants;
using Restia.Common.Shared.Entities;
using Restia.Common.Shared.Models;
using Restia.Common.Shared.Repository;

namespace Restia.Common.Identity.Repository
{
    public class UserRepository : BaseRepository<AppUser>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<UserGetDto>> GetPagedListAsync(SearchParam? searchParam, int page, int pageSize)
        {
            var query = Context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchParam?.SearchTerm))
            {
                var term = searchParam.SearchTerm.Trim();

                query = query.Where(u =>
                    (u.UserName != null && u.UserName.Contains(term)) ||
                    (u.DisplayName != null && u.DisplayName.Contains(term)) ||
                    (u.Email != null && u.Email.Contains(term)));
            }

            var totalCount = await query.CountAsync();

            query = searchParam?.OrderBy switch
            {
                AppConstants.UserNameAsc => query.OrderBy(u => u.UserName).ThenBy(u => u.Id),
                AppConstants.UserNameDesc => query.OrderByDescending(u => u.UserName).ThenBy(u => u.Id),
                AppConstants.EmailDesc => query.OrderByDescending(u => u.Email).ThenBy(u => u.Id),
                AppConstants.EmailAsc => query.OrderBy(u => u.Email).ThenBy(u => u.Id),
                _ => query.OrderBy(u => u.UserName).ThenBy(u => u.Id)
            };

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userIds = users.Select(u => u.Id).ToList();

            var rolesByUser = userIds.Count > 0
                ? await GetRoleNamesByUserIdsAsync(userIds)
                : new Dictionary<string, IList<string>>();

            var items = users.Select(u => new UserGetDto(
                u.Id,
                u.Email ?? string.Empty,
                u.UserName ?? string.Empty,
                Roles: rolesByUser.TryGetValue(u.Id, out var roles)
                    ? roles
                    : [],
                IsLockedOut: u.LockoutEnd.HasValue &&
                             u.LockoutEnd > DateTimeOffset.UtcNow
            )).ToList();

            return new PagedResult<UserGetDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<Dictionary<string, IList<string>>> GetRoleNamesByUserIdsAsync(IEnumerable<string> userIds)
        {
            var ids = userIds.ToList();

            var rows = await (
                from ur in Context.UserRoles
                join r in Context.Roles on ur.RoleId equals r.Id
                where ids.Contains(ur.UserId)
                select new { ur.UserId, RoleName = r.Name! }
            ).ToListAsync();

            return rows
                .GroupBy(x => x.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => (IList<string>)g.Select(x => x.RoleName).ToList());
        }
    }
}
