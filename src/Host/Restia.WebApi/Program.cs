using Restia.WebApi.Configurations;

namespace Restia.WebApi;

public class Program
{
	public static void Main(string[] args)
	{
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
