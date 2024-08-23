using Microsoft.Extensions.DependencyInjection;
using Restia.Infrastructure.Persistence.Settings;
using Serilog;

namespace Restia.Infrastructure.Persistence;
internal static class Startup
{
	/// <summary>The logger</summary>
	private static readonly ILogger _logger = Log.ForContext(typeof(Startup));

	/// <summary>
	/// Add persistence
	/// </summary>
	/// <param name="services">The services</param>
	/// <returns>A <see cref="IServiceCollection"/>.</returns>
	internal static IServiceCollection AddPersistence(this IServiceCollection services)
	{
		services.AddOptions<DatabaseSettings>()
			.BindConfiguration(nameof(DatabaseSettings))
			.PostConfigure(databaseSettings =>
			{
				_logger.Information("Current DB Provider: {dbProvider}", databaseSettings.DBProvider);
			})
			.ValidateDataAnnotations()
			.ValidateOnStart();

		return services;
	}
}
