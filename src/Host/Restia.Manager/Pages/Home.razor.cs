using Microsoft.AspNetCore.Components;

namespace Restia.Manager.Pages;

public partial class Home
{
	private string _baseUrl = string.Empty;

	[Inject]
	private HttpClient _httpClient { get; set; } = null!;

	// Always using parameterless ctor
	public Home()
	{
	}

	protected override void OnInitialized()
	{
		_baseUrl = _httpClient.BaseAddress!.ToString();
	}
}
