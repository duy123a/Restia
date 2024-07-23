using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Restia.Client.Infrastructure;

public static class Startup
{
	private const string ClientName = "Restia.API";

	public static IServiceCollection AddClientServices(this IServiceCollection services, IConfiguration config)
	{
		var apiBaseUrl = config.GetSection(ConfigNames.ApiBaseUrl)?.Value ?? string.Empty;

		return services
			// This will regist IHttpClientFactory to DI
			.AddHttpClient(ClientName, client =>
				{
					client.BaseAddress = new Uri(apiBaseUrl);
				})
				.Services
			.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient(ClientName));
	}
}
