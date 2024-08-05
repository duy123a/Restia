using Microsoft.AspNetCore.Components;

namespace Restia.Manager.Pages;

public partial class Home
{
	private string _result = string.Empty;

	[Inject]
	private HttpClient _httpClient { get; set; } = null!;

	// Always using parameterless ctor
	public Home()
	{
	}

	protected override async Task OnInitializedAsync()
	{
		_result = await _httpClient.GetStringAsync("api/weatherforecast");
	}
}
