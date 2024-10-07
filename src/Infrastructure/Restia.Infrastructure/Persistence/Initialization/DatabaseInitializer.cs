using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Restia.Infrastructure.Multitenancy.Context;
using Restia.Infrastructure.Multitenancy.Models;
using Restia.Infrastructure.Persistence.Initialization.Interfaces;
using Restia.Shared.Multitenancy;

namespace Restia.Infrastructure.Persistence.Initialization;

internal class DatabaseInitializer : IDatabaseInitializer
{
	/// <summary>The tenant DB context</summary>
	private readonly TenantDbContext _tenantDbContext;
	/// <summary>The service provider</summary>
	private readonly IServiceProvider _serviceProvider;
	/// <summary>The logger</summary>
	private readonly ILogger<DatabaseInitializer> _logger;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="tenantDbContext">The tenant DB context</param>
	/// <param name="serviceProvider">The service provider</param>
	/// <param name="logger">The logger</param>
	public DatabaseInitializer(
		TenantDbContext tenantDbContext,
		IServiceProvider serviceProvider,
		ILogger<DatabaseInitializer> logger)
	{
		_tenantDbContext = tenantDbContext;
		_serviceProvider = serviceProvider;
		_logger = logger;
	}

	/// <summary>
	/// Initialize databases async
	/// </summary>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A task</returns>
	public async Task InitializeDatabasesAsync(CancellationToken cancellationToken)
	{
		await InitializeTenantDbAsync(cancellationToken);

		foreach (var tenant in await _tenantDbContext.TenantInfo.ToListAsync(cancellationToken))
		{
			await InitializeApplicationDbForTenantAsync(tenant, cancellationToken);
		}
	}

	/// <summary>
	/// Initialize application db for tenant async
	/// </summary>
	/// <param name="tenant">The tenant</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A task</returns>
	public async Task InitializeApplicationDbForTenantAsync(RestiaTenantInfo tenant, CancellationToken cancellationToken)
	{
		// First create a new scope
		using var scope = _serviceProvider.CreateScope();

		// Then set current tenant temporarily so the right connectionstring is used (only in this scope)
		_serviceProvider.GetRequiredService<IMultiTenantContextSetter>()
			.MultiTenantContext = new MultiTenantContext<RestiaTenantInfo>()
			{
				TenantInfo = tenant
			};

		// Then run the initialization in the new scope
		await scope.ServiceProvider.GetRequiredService<ApplicationDbInitializer>()
			.InitializeAsync(cancellationToken);
	}

	/// <summary>
	/// Initialize tenant db async
	/// </summary>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A task</returns>
	private async Task InitializeTenantDbAsync(CancellationToken cancellationToken)
	{
		if (_tenantDbContext.Database.GetPendingMigrations().Any())
		{
			_logger.LogInformation("Applying Root Migrations.");
			await _tenantDbContext.Database.MigrateAsync(cancellationToken);
		}

		await SeedRootTenantAsync(cancellationToken);
	}

	/// <summary>
	/// Seed root tenant async
	/// </summary>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A task</returns>
	private async Task SeedRootTenantAsync(CancellationToken cancellationToken)
	{
		var rootTenant = await _tenantDbContext.TenantInfo.FindAsync(
			new object?[] { MultitenancyConstants.Root.Id },
			cancellationToken: cancellationToken);

		if (rootTenant == null)
		{
			var newRootTenant = new RestiaTenantInfo(
				MultitenancyConstants.Root.Id,
				MultitenancyConstants.Root.Name,
				string.Empty,
				MultitenancyConstants.Root.EmailAddress);

			newRootTenant.SetValidity(DateTime.UtcNow.AddYears(1));

			_tenantDbContext.TenantInfo.Add(newRootTenant);

			await _tenantDbContext.SaveChangesAsync(cancellationToken);
		}
		else if (rootTenant.ValidUpto < DateTime.UtcNow)
		{
			rootTenant.SetValidity(DateTime.UtcNow.AddYears(1));
			await _tenantDbContext.SaveChangesAsync(cancellationToken);
		}
	}
}
