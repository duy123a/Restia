using Restia.Infrastructure.Multitenancy.Models;

namespace Restia.Infrastructure.Persistence.Initialization.Interfaces;

internal interface IDatabaseInitializer
{
	/// <summary>
	/// Initialize databases async
	/// </summary>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A task</returns>
	Task InitializeDatabasesAsync(CancellationToken cancellationToken);

	/// <summary>
	/// Initialize application db for tenant async
	/// </summary>
	/// <param name="tenant">The tenant</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A task</returns>
	Task InitializeApplicationDbForTenantAsync(RestiaTenantInfo tenant, CancellationToken cancellationToken);
}
