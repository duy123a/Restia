using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Restia.Infrastructure.Identity.Models;
using Restia.Infrastructure.Multitenancy.Models;

namespace Restia.Infrastructure.Persistence.Context;
public abstract class BaseDbContext :
	MultiTenantIdentityDbContext<
		ApplicationUser,
		ApplicationRole,
		string,
		IdentityUserClaim<string>,
		IdentityUserRole<string>,
		IdentityUserLogin<string>,
		ApplicationRoleClaim,
		IdentityUserToken<string>>
{
	public BaseDbContext(
		IMultiTenantContextAccessor<RestiaTenantInfo> currentTenant,
		DbContextOptions options)
			: base(currentTenant, options)
	{
	}
}
