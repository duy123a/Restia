using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Restia.Infrastructure.Caching.Settings;

namespace Restia.Infrastructure.Caching;
internal static class Startup
{
	/// <summary>
	/// Add caching
	/// </summary>
	/// <param name="services">The services</param>
	/// <param name="config">The configuration</param>
	/// <returns>A <see cref="IServiceCollection"/>.</returns>
	internal static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration config)
	{
		var settings = config.GetSection(nameof(CacheSettings)).Get<CacheSettings>();
		if (settings == null)
			return services;

		// Due to OrchardCore.Localization require non-distributed cache, you need to add it anyway
		services.AddMemoryCache();

		return services;
	}
}
