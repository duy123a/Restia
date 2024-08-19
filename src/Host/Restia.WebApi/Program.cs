using Restia.Application;
using Restia.Infrastructure;
using Restia.Infrastructure.Common;
using Restia.Infrastructure.Logging.Serilog;
using Restia.WebApi.Configurations;
using Serilog;

namespace Restia.WebApi;

public class Program
{
	public static void Main(string[] args)
	{
		StaticLogger.EnsureInitialized();
		Log.Information("Server booting up...");

		try
		{
			// This method will load appsettings.json at default
			var builder = WebApplication.CreateBuilder(args);
			builder.AddConfigurations();
			builder.RegisterSerilog();
			builder.Services.AddControllers();
			builder.Services.AddInfrastructure(builder.Configuration);
			builder.Services.AddApplication();

			var app = builder.Build();

			app.UseInfrastructure(builder.Configuration);
			app.MapControllers();

			app.Run();
		}
		catch (Exception ex) when (ex is not HostAbortedException)
		{
			StaticLogger.EnsureInitialized();
			Log.Fatal(ex, "Unhandled exception");
		}
		finally
		{
			StaticLogger.EnsureInitialized();
			Log.Information("Server shutting down...");
			Log.CloseAndFlush();
		}
	}
}
