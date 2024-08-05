using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restia.Infrastructure.Cors;

namespace Restia.Infrastructure;

/// <summary>
/// Startup
/// </summary>
public static class Startup
{
	/// <summary>
	/// Add infrastructure
	/// </summary>
	/// <param name="services">The services</param>
	/// <param name="config">The configuration</param>
	/// <returns>A <see cref="IServiceCollection"/>.</returns>
	public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
	{
		return services
			.AddCorsPolicy(config);
	}

	/// <summary>
	/// Use infrastructure
	/// </summary>
	/// <param name="builder">The application builder</param>
	/// <param name="config">The configuration</param>
	/// <returns>A <see cref="IApplicationBuilder"/>.</returns>
	public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder builder, IConfiguration config)
	{
		return builder
			.UseCorsPolicy();
	}
}
