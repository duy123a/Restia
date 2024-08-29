using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Restia.Infrastructure.Common.Constants;
using Restia.Infrastructure.Persistence.Context;
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

		return services
			.AddDbContext<ApplicationDbContext>((serviceProvider, builder) =>
			{
				var databaseSettings = serviceProvider.GetRequiredService<IOptions<DatabaseSettings>>().Value;
				builder.UseDatabase(databaseSettings.DBProvider, databaseSettings.ConnectionString);
			});
	}

	/// <summary>
	/// Use database
	/// </summary>
	/// <param name="builder">The builder</param>
	/// <param name="dbProvider">The db provider</param>
	/// <param name="connectionString">The connection string</param>
	/// <returns>A <see cref="DbContextOptionsBuilder"/>.</returns>
	/// <exception cref="InvalidOperationException">A InvalidOperationException</exception>
	internal static DbContextOptionsBuilder UseDatabase(
		this DbContextOptionsBuilder builder,
		string dbProvider,
		string connectionString)
	{
		return dbProvider.ToLowerInvariant() switch
		{
			DbProviderKeys.Npgsql =>
				builder.UseNpgsql(connectionString, e => e.MigrationsAssembly("Restia.Migrators.PostgreSQL")),
			DbProviderKeys.SqlServer =>
				builder.UseSqlServer(connectionString, e =>
				{
					e.MigrationsAssembly("Restia.Migrators.MsSQL");
					e.UseCompatibilityLevel(150);
				}),
			_ => throw new InvalidOperationException($"DB Provider {dbProvider} is not supported."),
		};
	}
}
