using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Restia.Infrastructure.Persistence.Context;
public class ApplicationDbContext : BaseDbContext
{
	public ApplicationDbContext(
		ITenantInfo currentTenant,
		DbContextOptions options)
			: base(
				currentTenant,
				options)
	{

	}
}
