using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Restia.Common.Entities;
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

            identity.AddClaim(new Claim(Constants.IdentityConstants.DisplayName, user.DisplayName ?? string.Empty));

            return identity;
        }
    }
}
