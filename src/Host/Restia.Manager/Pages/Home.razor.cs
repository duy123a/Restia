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
		// Check your current localization
		//System.Globalization.CultureInfo.CurrentCulture;

		//_result = await _httpClient.GetStringAsync("api/v1/weatherforecast");
		_result = await Task.FromResult<string>("test");
	}
}
