using Restia.Application.Model.WeatherForecast;

namespace Restia.WebApi.Controllers.Example
{
	public class WeatherForecastController : VersionedApiController
	{
		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<WeatherForecastController> _logger;

		public WeatherForecastController(ILogger<WeatherForecastController> logger)
		{
			_logger = logger;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			_logger.LogInformation("Get information of weather forecast success");
			var query = new GetWeatherForecastRequest();
			var result = await Mediator.Send(query);
			return Ok(result);
		}
	}
}
