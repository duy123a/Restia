using Microsoft.Extensions.DependencyInjection;
using Restia.Application.Common.Interfaces;

namespace Restia.Infrastructure.Common;

/// <summary>
/// Startup
/// </summary>
internal static class Startup
{
	/// <summary>
	/// Add services
	/// </summary>
	/// <param name="services">The services</param>
	/// <returns>A <see cref="IServiceCollection"/>.</returns>
	internal static IServiceCollection AddServices(this IServiceCollection services)
	{
		// If you registered the same service and its implement both lifetime, you resolve the service with the last lifetime you registered
		// In this case it will be scoped one
		return services
			.AddServices(typeof(ITransientService), ServiceLifetime.Transient)
			.AddServices(typeof(IScopedService), ServiceLifetime.Scoped);
	}

	/// <summary>
	/// Add Services
	/// </summary>
	/// <param name="services">The services</param>
	/// <param name="interfaceType">The interface type</param>
	/// <param name="lifetime">The lifetime</param>
	/// <returns>A <see cref="IServiceCollection"/>.</returns>
	internal static IServiceCollection AddServices(
		this IServiceCollection services,
		Type interfaceType,
		ServiceLifetime lifetime)
	{
		var interfaceTypes = AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(type => interfaceType.IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
			.Select(type =>
				new
				{
					Service = type.GetInterfaces().FirstOrDefault(),
					Implementation = type
				})
			.Where(type => type.Service is not null && interfaceType.IsAssignableFrom(type.Service));

		foreach (var type in interfaceTypes)
		{
			services.AddService(type.Service!, type.Implementation, lifetime);
		}

		return services;
	}

	/// <summary>
	/// Add service
	/// </summary>
	/// <param name="services">The services</param>
	/// <param name="serviceType">The service type</param>
	/// <param name="implementationType">The implementation type</param>
	/// <param name="lifetime">The lifetime</param>
	/// <returns>A <see cref="IServiceCollection"/>.</returns>
	/// <exception cref="ArgumentException">A ArgumentException</exception>
	internal static IServiceCollection AddService(
		this IServiceCollection services,
		Type serviceType,
		Type implementationType,
		ServiceLifetime lifetime)
	{
		return lifetime switch
		{
			ServiceLifetime.Transient => services.AddTransient(serviceType, implementationType),
			ServiceLifetime.Scoped => services.AddScoped(serviceType, implementationType),
			ServiceLifetime.Singleton => services.AddSingleton(serviceType, implementationType),
			_ => throw new ArgumentException("Invalid lifeTime", nameof(lifetime))
		};
	}
}
