using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Restia.Application.Common.Interfaces;
using Restia.Infrastructure.Auth.Interfaces;
using Restia.Infrastructure.Auth.Jwt;
using Restia.Infrastructure.Auth.Middleware;
using Restia.Infrastructure.Auth.Models;
using Restia.Infrastructure.Auth.Settings;
using Restia.Infrastructure.Identity;
using Restia.Infrastructure.Permissions;

namespace Restia.Infrastructure.Auth;

public static class Startup
{
	/// <summary>
	/// Add auth
	/// </summary>
	/// <param name="services">The services</param>
	/// <returns>A <see cref="IServiceCollection"/>.</returns>
	internal static IServiceCollection AddAuth(this IServiceCollection services)
	{
		services
			.AddCurrentUser()
			.AddPermissions()

			// Must add identity before adding auth!
			.AddIdentity();

		//services.Configure<SecuritySettings>(config.GetSection(nameof(SecuritySettings)));
		services.AddOptions<SecuritySettings>().BindConfiguration(nameof(SecuritySettings));

		return services.AddJwtAuth();
	}

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

	/// <summary>
	/// Add permissions
	/// </summary>
	/// <param name="services">The services</param>
	/// <returns>A <see cref="IServiceCollection"/>.</returns>
	private static IServiceCollection AddPermissions(this IServiceCollection services) =>
		services
			.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>()
			.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
}
