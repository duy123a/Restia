using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Restia.Infrastructure.Multitenancy.Models;
using Restia.Infrastructure.Persistence.Settings;

namespace Restia.Infrastructure.Persistence.Context;
public class ApplicationDbContext : BaseDbContext
{
	// IMultiTenantContext, ITenantInfo is not imposed in the latest version of Finbuckle.MultiTenant
	public ApplicationDbContext(
		IMultiTenantContextAccessor<RestiaTenantInfo> currentMultiTenantContextAccessor,
		DbContextOptions options,
		IOptions<DatabaseSettings> dbSettings)
			: base(
				currentMultiTenantContextAccessor,
				options,
				dbSettings)
	{
	}
}
