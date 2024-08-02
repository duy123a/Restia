using Restia.Infrastructure.Common;
using Restia.WebApi.Configurations;
using Serilog;

namespace Restia.WebApi;

public class Program
{
	public static void Main(string[] args)
	{
		StaticLogger.EnsureInitialized();
		Log.Information("Server Booting Up...");

		// This method will load appsettings.json at default
		var builder = WebApplication.CreateBuilder(args);
		builder.AddConfigurations();

		// Add services to the container.
		builder.Services.AddControllers();

		var app = builder.Build();

		// Configure the HTTP request pipeline.
		app.MapControllers();

		app.Run();
	}
}
