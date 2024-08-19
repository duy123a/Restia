using System.Reflection;
using Asp.Versioning;
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
			.AddCorsPolicy(config)
			.AddApiVersioning()
			.AddMediatR();
	}

	/// <summary>
	/// Add api versioning
	/// </summary>
	/// <param name="services">The services</param>
	/// <returns>A <see cref="IServiceCollection"/>.</returns>
	private static IServiceCollection AddApiVersioning(this IServiceCollection services)
	{
		var apiVersioningBuilder = services.AddApiVersioning(options =>
		{
			options.DefaultApiVersion = new ApiVersion(1, 0);
			options.AssumeDefaultVersionWhenUnspecified = true;
			options.ReportApiVersions = true;
		});

		// This is only useful for api doc/swagger
		apiVersioningBuilder.AddApiExplorer(options =>
		{
			options.GroupNameFormat = "'v'VVV";
			options.SubstituteApiVersionInUrl = true;
		});

		return services;
	}

	/// <summary>
	/// Add mediatR
	/// </summary>
	/// <param name="services">The services</param>
	/// <returns>A <see cref="IServiceCollection"/>.</returns>
	private static IServiceCollection AddMediatR(this IServiceCollection services)
	{
		// Will find an assembly contain this class, kinda hack code so don't use it
		// services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetWeatherForecastRequest>());

		services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
		return services;
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
