namespace Restia.Infrastructure.Localization.Settings;

public class LocalizationSettings
{
	/// <summary>Get and set enable localization</summary>
	public bool? EnableLocalization { get; set; }
	/// <summary>Get and set resources path</summary>
	public string? ResourcesPath { get; set; }
	/// <summary>Get and set supported cultures</summary>
	public string[]? SupportedCultures { get; set; }
	/// <summary>Get and set default request culture</summary>
	public string? DefaultRequestCulture { get; set; }
	/// <summary>Get and set fallback to parent</summary>
	public bool? FallbackToParent { get; set; }
}
