namespace Restia.Infrastructure.Auth.Settings;

/// <summary>
/// Security Settings
/// </summary>
public class SecuritySettings
{
	/// <summary>Get and set provider</summary>
	public string? Provider { get; set; }
	/// <summary>Get and set require confirmed account</summary>
	public bool RequireConfirmedAccount { get; set; }
}
