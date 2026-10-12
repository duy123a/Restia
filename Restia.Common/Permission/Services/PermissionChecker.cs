using Restia.Common.Identity.Extensions;
using Restia.Common.Permission.Constants;
using Restia.Common.Permission.Services.Interfaces;
using System.Security.Claims;

namespace Restia.Common.Permission.Services
{
    public class PermissionChecker : IPermissionChecker
    {
        private readonly IPermissionService _permissionService;

        public PermissionChecker(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        public async Task<bool> HasPermissionAsync(
            ClaimsPrincipal user,
            string policyName)
        {
            if (user.Identity?.IsAuthenticated != true)
                return false;

            if (user.IsSuperAdmin())
                return true;

            var parts = policyName.Split(':');

            if (parts.Length < 2 || parts.Length > 3 ||
                parts.Any(string.IsNullOrWhiteSpace))
            {
                throw new ArgumentException(
                    $"Invalid permission policy: {policyName}",
                    nameof(policyName));
            }

            var screen = parts[1];
            FormAuthoritySec? requiredAuthority = null;

            if (parts.Length == 3)
            {
                if (!Enum.TryParse<FormAuthoritySec>(
                        parts[2], true, out var authority)
                    || !Enum.IsDefined(typeof(FormAuthoritySec), authority))
                {
                    throw new ArgumentException(
                        $"Invalid form authority: {parts[2]}",
                        nameof(policyName));
                }

                requiredAuthority = authority;
            }

            var authoritySec = await _permissionService.GetAuthoritySecAsync(
                user.GetLoginId(),
                user.GetRoles(),
                screen);

            if (requiredAuthority == null)
            {
                return authoritySec != FormAuthoritySec.None
                    && authoritySec != FormAuthoritySec.Unavailable;
            }

            return authoritySec == requiredAuthority;
        }
    }
}