using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Restia.Client.Infrastructure;

namespace Restia.Manager;

public class Program
{
	public static async Task Main(string[] args)
	{
		// This method will load appsettings.json at default
		// It uses this env var ASPNETCORE_ENVIRONMENT to decide which appsettings to load
		var builder = WebAssemblyHostBuilder.CreateDefault(args);
		builder.RootComponents.Add<App>("#app");
		builder.RootComponents.Add<HeadOutlet>("head::after");

		builder.Services.AddClientServices(builder.Configuration);
		builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

		var host = builder.Build();

		// Set culture
		//var culture = new CultureInfo("en-US"); // You can test different cultures here
		//CultureInfo.DefaultThreadCurrentCulture = culture;
		//CultureInfo.DefaultThreadCurrentUICulture = culture;

		await host.RunAsync();
	}
}
