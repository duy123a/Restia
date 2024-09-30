using System.Reflection;
using Asp.Versioning;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restia.Infrastructure.Auth;
using Restia.Infrastructure.Common;
using Restia.Infrastructure.Cors;
using Restia.Infrastructure.Multitenancy;
using Restia.Infrastructure.Multitenancy.Services;
using Restia.Infrastructure.Persistence;

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
			.AddApiVersioning()
			.AddAuth()
			.AddCorsPolicy(config)
			.AddHealthCheck()
			.AddMediatR()
			.AddMultitenancy()
			.AddPersistence()
			.AddServices();
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

		// This is only useful for open api/nswag
		apiVersioningBuilder.AddApiExplorer(options =>
		{
			options.SubstituteApiVersionInUrl = true;
		});

		return apiVersioningBuilder.Services;
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
	/// Add custom health check
	/// </summary>
	/// <param name="services">The services</param>
	/// <returns>A <see cref="IServiceCollection"/>.</returns>
	private static IServiceCollection AddHealthCheck(this IServiceCollection services)
	{
		return services
			.AddHealthChecks()
			.AddCheck<TenantHealthCheck>("Tenant")
			.Services;
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
			.UseCorsPolicy()
			.UseMultiTenancy();
	}

	/// <summary>
	/// Map endpoints
	/// </summary>
	/// <param name="builder">The endpoint route builder</param>
	/// <returns>A <see cref="IEndpointRouteBuilder"/>.</returns>
	public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
	{
		builder.MapControllers();
		builder.MapHealthCheck();
		return builder;
	}

	/// <summary>
	/// Map health check
	/// </summary>
	/// <param name="endpoints">The endpoint route builder</param>
	/// <returns>A <see cref="IEndpointConventionBuilder"/>.</returns>
	private static IEndpointConventionBuilder MapHealthCheck(this IEndpointRouteBuilder endpoints) =>
		endpoints.MapHealthChecks("/api/health");
}
