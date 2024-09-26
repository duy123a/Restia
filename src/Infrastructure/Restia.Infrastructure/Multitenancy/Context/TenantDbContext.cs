using Finbuckle.MultiTenant.EntityFrameworkCore.Stores.EFCoreStore;
using Microsoft.EntityFrameworkCore;
using Restia.Infrastructure.Multitenancy.Models;
using Restia.Infrastructure.Persistence.Configuration;

namespace Restia.Infrastructure.Multitenancy.Context;

public class TenantDbContext : EFCoreStoreDbContext<RestiaTenantInfo>
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="options">The options</param>
	public TenantDbContext(DbContextOptions<TenantDbContext> options)
		: base(options)
	{
		// Allow working with timestamps without time zones.
		AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
	}

	/// <summary>
	/// On model creating
	/// </summary>
	/// <param name="modelBuilder">The model builder</param>
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Customize table name from RestiaTenantInfo to Tenants
		modelBuilder.Entity<RestiaTenantInfo>().ToTable("Tenants", SchemaNames.MultiTenancy);
	}
}
