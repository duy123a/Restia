using Restia.Common.Permission.Constants;
using Restia.Common.Permission.Repository.Interfaces;
using Restia.Common.Permission.Services.Interfaces;

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
    }
}
