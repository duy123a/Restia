using Restia.Common.Identity.Extensions;
using Restia.Common.Permission.Constants;
using Restia.Common.Permission.Repository.Interfaces;
using Restia.Common.Permission.Services.Interfaces;
using System.Security.Claims;

namespace Restia.Common.Permission.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IUserPermissionRepository _userPermissionRepository;
        private readonly IRolePermissionRepository _rolePermissionRepository;

        public PermissionService(
            IUserPermissionRepository userPermissionRepository,
            IRolePermissionRepository rolePermissionRepository)
        {
            _userPermissionRepository = userPermissionRepository;
            _rolePermissionRepository = rolePermissionRepository;
        }

        public async Task<FormAuthoritySec> GetAuthoritySecAsync(string userId, IEnumerable<string> roleIds, string formId)
        {
            var userAuthority = await _userPermissionRepository.GetByIdAsync(new object[] { userId, formId });
            if (userAuthority != null && userAuthority.FormAuthoritySec != FormAuthoritySec.None)
            {
                return userAuthority.FormAuthoritySec;
            }

            var authorities = new List<FormAuthoritySec>();

            foreach (var roleId in roleIds.Distinct())
            {
                var roleAuthority = await _rolePermissionRepository
                    .GetByIdAsync(new object[] { roleId, formId });

                if (roleAuthority != null &&
                    roleAuthority.FormAuthoritySec != FormAuthoritySec.None)
                {
                    authorities.Add(roleAuthority.FormAuthoritySec);
                }
            }

            if (authorities.Contains(FormAuthoritySec.Unavailable))
            {
                return FormAuthoritySec.Unavailable;
            }

            if (authorities.Contains(FormAuthoritySec.Available))
            {
                return FormAuthoritySec.Available;
            }

            if (authorities.Contains(FormAuthoritySec.ReferablePrintable))
            {
                return FormAuthoritySec.ReferablePrintable;
            }

            return FormAuthoritySec.Unavailable;
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

            var authoritySec = await GetAuthoritySecAsync(
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
