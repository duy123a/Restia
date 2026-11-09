using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Restia.Common.Identity.Constants;
using Restia.Common.Identity.Entities;
using Restia.Common.Tenant.Constants;
using System.Security.Claims;

namespace Restia.Common.Identity.Services
{
    public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<AppUser, AppRole>
    {
        public AppUserClaimsPrincipalFactory(
            UserManager<AppUser> userManager,
            RoleManager<AppRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, roleManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(AppUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            var schemaId = string.IsNullOrWhiteSpace(user.SchemaId)
                ? TenantConstants.DefaultTenantId
                : user.SchemaId;

            identity.AddClaim(new Claim(CustomClaimTypes.DisplayName, user.DisplayName ?? string.Empty));
            identity.AddClaim(new Claim(CustomClaimTypes.SchemaId, schemaId));

            return identity;
        }
    }
}
