using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restia.Infrastructure.Multitenancy.Models;
using Restia.Infrastructure.Persistence.Context;

namespace Restia.Infrastructure.Persistence.Initialization;

internal class ApplicationDbInitializer
{
	/// <summary>The db context</summary>
	private readonly ApplicationDbContext _dbContext;
	/// <summary>The current tenant</summary>
	private readonly ITenantInfo _currentTenant;
	/// <summary>The db seeder</summary>
	private readonly ApplicationDbSeeder _dbSeeder;
	/// <summary>The logger</summary>
	private readonly ILogger<ApplicationDbInitializer> _logger;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="dbContext">The db context</param>
	/// <param name="currentMultiTenantContextAccessor">The current multi tenant context accessor</param>
	/// <param name="dbSeeder">The db seeder</param>
	/// <param name="logger">The logger</param>
	public ApplicationDbInitializer(
		ApplicationDbContext dbContext,
		IMultiTenantContextAccessor<RestiaTenantInfo> currentMultiTenantContextAccessor,
		ApplicationDbSeeder dbSeeder,
		ILogger<ApplicationDbInitializer> logger)
	{
		_dbContext = dbContext;
		_currentTenant = currentMultiTenantContextAccessor.MultiTenantContext.TenantInfo!;
		_dbSeeder = dbSeeder;
		_logger = logger;
	}

	/// <summary>
	/// Initialize async
	/// </summary>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A task</returns>
	public async Task InitializeAsync(CancellationToken cancellationToken)
	{
		if (_dbContext.Database.GetMigrations().Any())
		{
			if ((await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken)).Any())
			{
				_logger.LogInformation("Applying Migrations for '{tenantId}' tenant.", _currentTenant.Id);
				await _dbContext.Database.MigrateAsync(cancellationToken);
			}

			if (await _dbContext.Database.CanConnectAsync(cancellationToken))
			{
				_logger.LogInformation("Connection to {tenantId}'s Database Succeeded.", _currentTenant.Id);

				await _dbSeeder.SeedDatabaseAsync(_dbContext, cancellationToken);
			}
		}
	}
}
