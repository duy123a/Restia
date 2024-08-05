using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restia.Infrastructure.Cors.Settings;

namespace Restia.Infrastructure.Cors;

/// <summary>
/// Startup
/// </summary>
internal static class Startup
{
	/// <summary>Cors Policy</summary>
	private const string CorsPolicy = nameof(CorsPolicy);

	/// <summary>
	/// Add cors policy
	/// </summary>
	/// <param name="services">The services</param>
	/// <param name="config">The configuration</param>
	/// <returns>A <see cref="IServiceCollection"/>.</returns>
	internal static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration config)
	{
		var corsSettings = config.GetSection(nameof(CorsSettings)).Get<CorsSettings>();
		if (corsSettings == null)
			return services;

		var origins = new List<string>();
		if (corsSettings.Blazor is not null)
		{
			origins.AddRange(corsSettings.Blazor.Split(';', StringSplitOptions.RemoveEmptyEntries));
		}

		if (corsSettings.React is not null)
		{
			origins.AddRange(corsSettings.React.Split(';', StringSplitOptions.RemoveEmptyEntries));
		}

		return services.AddCors(opt =>
			opt.AddPolicy(CorsPolicy, policy =>
				policy.AllowAnyHeader()
					.AllowAnyMethod()
					.AllowCredentials()
					.WithOrigins(origins.ToArray())));
	}

	/// <summary>
	/// Use cors policy
	/// </summary>
	/// <param name="app">The application builder</param>
	/// <returns>A <see cref="IApplicationBuilder"/>.</returns>
	internal static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app) =>
		app.UseCors(CorsPolicy);
}
