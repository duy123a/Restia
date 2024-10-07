using Restia.Infrastructure.Persistence.Initialization.Interfaces;

namespace Restia.Infrastructure.Persistence.Initialization;

internal class CustomSeederRunner
{
	/// <summary>The seeders</summary>
	private readonly IEnumerable<ICustomSeeder> _seeders;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="seeders">Seeders</param>
	public CustomSeederRunner(IEnumerable<ICustomSeeder> seeders) =>
		_seeders = seeders;

	/// <summary>
	/// Run seeders async
	/// </summary>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A task</returns>
	public async Task RunSeedersAsync(CancellationToken cancellationToken)
	{
		foreach (var seeder in _seeders)
		{
			await seeder.InitializeAsync(cancellationToken);
		}
	}
}
