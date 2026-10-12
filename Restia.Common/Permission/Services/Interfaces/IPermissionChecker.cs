using System.Security.Claims;

namespace Restia.Common.Permission.Services.Interfaces
{
    public interface IPermissionChecker
    {
        Task<bool> HasPermissionAsync(
            ClaimsPrincipal user,
            string policyName);
    }
}
