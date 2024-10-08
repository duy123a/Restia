namespace Restia.WebApi.Configurations;

/// <summary>
/// Startup
/// </summary>
internal static class Startup
{
	/// <summary>
	/// Add configurations to <see cref="WebApplicationBuilder"/>
	/// </summary>
	/// <param name="builder">A web application builder</param>
	/// <returns><see cref="WebApplicationBuilder"/></returns>
	internal static WebApplicationBuilder AddConfigurations(this WebApplicationBuilder builder)
	{
		const string configurationsDirectory = "Configurations";
		var env = builder.Environment;

		// Need to explicitly add appsetting.json and environment variables back since they will be overwrited
		builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
			.AddJsonFile($"{configurationsDirectory}/cors.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"{configurationsDirectory}/cors.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
			.AddJsonFile($"{configurationsDirectory}/logger.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"{configurationsDirectory}/logger.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
			.AddJsonFile($"{configurationsDirectory}/database.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"{configurationsDirectory}/database.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
			.AddJsonFile($"{configurationsDirectory}/security.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"{configurationsDirectory}/security.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
			.AddJsonFile($"{configurationsDirectory}/localization.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"{configurationsDirectory}/localization.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
			.AddJsonFile($"{configurationsDirectory}/cache.json", optional: false, reloadOnChange: true)
			.AddJsonFile($"{configurationsDirectory}/cache.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
			.AddEnvironmentVariables();

		return builder;
	}
}
