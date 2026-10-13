using Restia.Common.Identity.Models;
using Restia.Common.Shared.Models;

namespace Restia.Common.Identity.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResult<UserGetDto>> GetPagedListAsync(string? search, int page, int pageSize);
        Task<UserUpdateDto?> GetInfoForUpdateAsync(string id);
        Task<IReadOnlyList<RoleOptionDto>> GetAllRoleOptionsAsync();
        Task<ServiceResult> CreateAsync(UserCreateDto dto);
        Task<ServiceResult> UpdateAsync(UserUpdateDto dto);
        Task<ServiceResult> DeleteAsync(string id, string currentUserId);
    }
}
