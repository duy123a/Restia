using Restia.Common.Permission.Constants;
using Restia.Common.Permission.Models;
using Restia.Common.Shared.Models;
using System.Security.Claims;

namespace Restia.Common.Permission.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<FormAuthoritySec> GetAuthoritySecAsync(string userId, IEnumerable<string> roleNames, string formCode);

        Task<bool> HasPermissionAsync(ClaimsPrincipal user, string policyName);

        Task<IReadOnlyList<PermissionTargetOptionDto>> GetTargetOptionsAsync(PermissionTargetType targetType);

        Task<PermissionMatrixDto?> GetMatrixAsync(PermissionTargetType targetType, string targetId);

        Task<ServiceResult> SaveMatrixAsync(
            PermissionTargetType targetType,
            string targetId,
            IReadOnlyList<FormPermissionDto> items);
    }
}
