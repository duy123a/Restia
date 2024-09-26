using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Restia.Application.Common.Interfaces;
using Restia.Infrastructure.Auth.Interfaces;
using Restia.Infrastructure.Auth.Middleware;
using Restia.Infrastructure.Auth.Models;

namespace Restia.Infrastructure.Auth;

public static class Startup
{
	/// <summary>
	/// Add current user
	/// </summary>
	/// <param name="services">The services</param>
	/// <returns>A <see cref="IServiceCollection"/>.</returns>
	private static IServiceCollection AddCurrentUser(this IServiceCollection services)
	{
		return services
			// If your middleware implement IMiddleware, then you need to regist it manually to DI
			.AddScoped<CurrentUserMiddleware>()
			.AddScoped<ICurrentUser, CurrentUser>()
			.AddScoped(sp => (ICurrentUserInitializer)sp.GetRequiredService<ICurrentUser>());
	}

	/// <summary>
	/// Use current user
	/// </summary>
	/// <param name="app">The application builder</param>
	/// <returns>A <see cref="IApplicationBuilder"/>.</returns>
	internal static IApplicationBuilder UseCurrentUser(this IApplicationBuilder app) =>
		app.UseMiddleware<CurrentUserMiddleware>();
}
