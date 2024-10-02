using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Restia.Application;
/// <summary>
/// Startup
/// </summary>
public static class Startup
{
	/// <summary>
	/// Add application
	/// </summary>
	/// <param name="services">The services</param>
	/// <returns>IServiceCollection</returns>
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		var assembly = Assembly.GetExecutingAssembly();
		return services
			// Need to hook FluentValidation into the MediatR pipeline automatically otherwise it won't work
			// The validation need to implement AbstractValidator<T>
			.AddValidatorsFromAssembly(assembly)
			.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
	}
}
