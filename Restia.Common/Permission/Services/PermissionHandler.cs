using Microsoft.AspNetCore.Authorization;
using Restia.Common.Permission.Requirements;
using Restia.Common.Permission.Services.Interfaces;

namespace Restia.Common.Permission.Services
{
    internal class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IPermissionService _permissionService;

        public PermissionHandler(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            if (context.User?.Identity?.IsAuthenticated != true)
            {
                context.Fail();
                return;
            }

            if (await _permissionService.HasPermissionAsync(context.User, requirement.PolicyName))
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
        }
    }
}
