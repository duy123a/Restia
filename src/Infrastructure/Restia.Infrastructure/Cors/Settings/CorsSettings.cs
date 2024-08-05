namespace Restia.Infrastructure.Cors.Settings;

/// <summary>
/// Cors Settings
/// </summary>
public class CorsSettings
{
	/// <summary>Get and set cors settings for Blazor</summary>
	public string? Blazor { get; set; }
	/// <summary>Get and set cors settings for React</summary>
	public string? React { get; set; }
}
