using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Restia.Infrastructure.Identity.Models;
using Restia.Infrastructure.Multitenancy.Models;
using Restia.Infrastructure.Persistence.Settings;

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
	/// <summary>The current user</summary>
	protected readonly RestiaTenantInfo _currentTenant;
	/// <summary>The db settings</summary>
	private readonly DatabaseSettings _dbSettings;

	public BaseDbContext(
		IMultiTenantContextAccessor<RestiaTenantInfo> currentMultiTenantContextAccessor,
		DbContextOptions options,
		IOptions<DatabaseSettings> dbSettings)
			: base(currentMultiTenantContextAccessor, options)
	{
		_currentTenant = currentMultiTenantContextAccessor.MultiTenantContext.TenantInfo!;
		_dbSettings = dbSettings.Value;
	}

	/// <summary>
	/// On configuring
	/// </summary>
	/// <param name="optionsBuilder">The option builder</param>
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		// TODO: We want this only for development probably... maybe better make it configurable in logger.json config?
		optionsBuilder.EnableSensitiveDataLogging();

		// If you want to see the sql queries that efcore executes:

		// Uncomment the next line to see them in the output window of visual studio
		// optionsBuilder.LogTo(m => System.Diagnostics.Debug.WriteLine(m), Microsoft.Extensions.Logging.LogLevel.Information);

		// Or uncomment the next line if you want to see them in the console
		// optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);

		if (!string.IsNullOrWhiteSpace(_currentTenant.ConnectionString))
		{
			optionsBuilder.UseDatabase(_dbSettings.DBProvider, _currentTenant.ConnectionString);
		}
	}
}
