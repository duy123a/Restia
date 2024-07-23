namespace Restia.WebApi.Configurations;

internal static class Startup
{
	internal static WebApplicationBuilder AddConfigurations(this WebApplicationBuilder builder)
	{
		const string configurationsDirectory = "Configurations";
		var env = builder.Environment;

		// Need to explicitly add appsetting.json and environment variables back since they will be overwrited
		builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
			.AddJsonFile($"{configurationsDirectory}/cors.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"{configurationsDirectory}/cors.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
			.AddEnvironmentVariables();

		return builder;
	}
}
