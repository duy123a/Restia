using MediatR;

namespace Restia.Application.Model.WeatherForecast;

public class GetWeatherForecastRequest : IRequest<IEnumerable<Restia.Domain.Model.WeatherForecast>>
{
	public GetWeatherForecastRequest(Guid id)
	{
		Id = id;
	}
	public GetWeatherForecastRequest()
	{
	}
	public Guid Id { get; set; }
}

public class GetWeatherForecastRequestHandler : IRequestHandler<GetWeatherForecastRequest, IEnumerable<Restia.Domain.Model.WeatherForecast>>
{
	public static readonly string[] Summaries = new[]
	{
		"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
	};

	public Task<IEnumerable<Domain.Model.WeatherForecast>> Handle(GetWeatherForecastRequest request, CancellationToken cancellationToken)
	{
		var forecasts = Enumerable.Range(1, 5).Select(index => new Domain.Model.WeatherForecast
		{
			Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
			TemperatureC = Random.Shared.Next(-20, 55),
			Summary = Summaries[Random.Shared.Next(Summaries.Length)]
		})
		.ToArray();

		return Task.FromResult<IEnumerable<Domain.Model.WeatherForecast>>(forecasts);
	}
}
