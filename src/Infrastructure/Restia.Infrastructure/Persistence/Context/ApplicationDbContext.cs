using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Restia.Infrastructure.Multitenancy.Models;

namespace Restia.Infrastructure.Persistence.Context;
public class ApplicationDbContext : BaseDbContext
{
	public ApplicationDbContext(
		IMultiTenantContextAccessor<RestiaTenantInfo> currentTenant,
		DbContextOptions options)
			: base(
				currentTenant,
				options)
	{
	}
}
