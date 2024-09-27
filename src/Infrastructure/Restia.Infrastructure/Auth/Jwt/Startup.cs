using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Restia.Infrastructure.Auth.Jwt.Configure;
using Restia.Infrastructure.Auth.Jwt.Settings;
using Restia.Shared.Authorization.Jwt;

namespace Restia.Infrastructure.Auth.Jwt;

internal static class Startup
{
	/// <summary>
	/// Add Jwt auth
	/// </summary>
	/// <param name="services">The services</param>
	/// <returns>A result is <see cref="IServiceCollection"/>.</returns>
	internal static IServiceCollection AddJwtAuth(this IServiceCollection services)
	{
		services
			.AddOptions<JwtSettings>()
			.BindConfiguration(nameof(JwtSettings))
			.PostConfigure(jwtSettings =>
			{
				var jwtSecretKey = Environment.GetEnvironmentVariable(CustomJwtConstants.JwtSecretKey);
				if (!string.IsNullOrEmpty(jwtSecretKey))
				{
					jwtSettings.Key = jwtSecretKey;
				}
			})
			.ValidateDataAnnotations()
			.ValidateOnStart();

		// IConfigureOptions usually used to create the custom options, which is different than IOptions which used to read directly from config file without customization
		services.AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

		return services;
	}
}
