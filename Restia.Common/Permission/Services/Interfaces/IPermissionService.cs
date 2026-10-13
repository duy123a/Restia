using Restia.Common.Permission.Constants;
using System.Security.Claims;

namespace Restia.Common.Permission.Services.Interfaces
{
    public interface IPermissionService
    {
        Task<FormAuthoritySec> GetAuthoritySecAsync(string userId, IEnumerable<string> roleIds, string formId);

        Task<bool> HasPermissionAsync(ClaimsPrincipal user, string policyName);
    }
}
