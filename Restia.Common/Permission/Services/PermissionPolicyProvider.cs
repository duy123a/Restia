using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Restia.Common.Permission.Requirements;

namespace Restia.Common.Permission.Services
{
    internal class PermissionPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (!policyName.Contains(PermissionRequirement.SEPARATOR))
                return await base.GetPolicyAsync(policyName);

            var policy = new AuthorizationPolicyBuilder();
            policy.RequireAuthenticatedUser();
            policy.AddRequirements(new PermissionRequirement(policyName));
            return policy.Build();
        }
    }
}
