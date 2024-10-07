namespace Restia.Infrastructure.Persistence.Initialization.Interfaces;

public interface ICustomSeeder
{
	/// <summary>
	/// Initialize async
	/// </summary>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A Task</returns>
	Task InitializeAsync(CancellationToken cancellationToken);
}
