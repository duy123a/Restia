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

		var host = builder.Build();
		await host.RunAsync();
	}
}
