namespace Restia.Client.Infrastructure.Common.Constants;

/// <summary>
/// Language code
/// </summary>
/// <param name="Code">The code</param>
/// <param name="DisplayName">The display name</param>
/// <param name="IsRTL">Is RTL</param>
/// <remarks>RTL (Right-to-Left) refers to the writing direction of languages such as Arabic or Hebrew, where text flows from the right side of the page to the left.</remarks>
public record LanguageCode(string Code, string DisplayName, bool IsRTL = false);

/// <summary>
/// Localization Constants
/// </summary>
public static class LocalizationConstants
{
	/// <summary>
	/// Supported languages
	/// </summary>
	public static readonly LanguageCode[] SupportedLanguages =
	{
		new("en-US", "English"),
		new("fr-FR", "French"),
	};
}
