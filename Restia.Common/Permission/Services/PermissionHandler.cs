using Microsoft.AspNetCore.Authorization;
using Restia.Common.Identity.Extensions;
using Restia.Common.Permission.Constants;
using Restia.Common.Permission.Requirements;
using Restia.Common.Permission.Services.Interfaces;

namespace Restia.Common.Permission.Services
{
    internal class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private IPermissionService _permissionService;

        public PermissionHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (!(context.User?.Identity?.IsAuthenticated ?? false))
            {
                context.Fail();
                return;
            }

            if (context.User.IsSuperAdmin())
            {
                context.Succeed(requirement);
                return;
            }

            var userId = context.User.GetLoginId();
            var userRoles = context.User.GetRoles();

            var formAuthoritySec = await _permissionService.GetAuthoritySecAsync(userId, userRoles, requirement.Screen);

            // =============================
            // 1. Controller-level check
            // =============================
            // Policy: "Module:Screen"
            if (requirement.FormAuthoritySec == null)
            {
                // Check FormAuthoritySec
                if (formAuthoritySec != FormAuthoritySec.Unavailable &&
                    formAuthoritySec != FormAuthoritySec.None)
                {
                    context.Succeed(requirement);
                    return;
                }

                context.Fail();
                return;
            }

            // =============================
            // 2. Action-level check
            // =============================
            // Policy: "Module:Screen:FormAuthoritySec"
            if (formAuthoritySec == requirement.FormAuthoritySec)
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
            return;
        }
    }
}
