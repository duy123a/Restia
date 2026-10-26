using Restia.Common.Identity.Models;
using Restia.Common.Shared.Entities;
using Restia.Common.Shared.Models;
using Restia.Common.Shared.Repository.Interfaces;

namespace Restia.Common.Identity.Repository.Interfaces
{
    public interface IUserRepository : IRepository<AppUser>
    {
        Task<PagedResult<UserGetDto>> GetPagedListAsync(SearchParam? searchParam, int page, int pageSize);

        Task<Dictionary<string, IList<string>>> GetRoleNamesByUserIdsAsync(IEnumerable<string> userIds);
    }
}
