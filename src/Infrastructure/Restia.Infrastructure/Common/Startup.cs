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
					Services = type.GetInterfaces().Where(i => interfaceType.IsAssignableFrom(i)),
					Implementation = type
				})
			.Where(result => result.Services.Count() == 1);

		foreach (var type in interfaceTypes)
		{
			services.AddService(type.Services.First(), type.Implementation, lifetime);
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
