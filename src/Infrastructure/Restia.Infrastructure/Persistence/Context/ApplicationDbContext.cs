using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Restia.Infrastructure.Multitenancy.Models;

namespace Restia.Infrastructure.Persistence.Context;
public class ApplicationDbContext : BaseDbContext
{
	// IMultiTenantContext, ITenantInfo is not imposed in the latest version of Finbuckle.MultiTenant
	public ApplicationDbContext(
		IMultiTenantContextAccessor<RestiaTenantInfo> currentMultiTenantContextAccessor,
		DbContextOptions options)
			: base(
				currentMultiTenantContextAccessor,
				options)
	{
	}
}
