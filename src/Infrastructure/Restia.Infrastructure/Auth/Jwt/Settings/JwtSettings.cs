using System.ComponentModel.DataAnnotations;

namespace Restia.Infrastructure.Auth.Jwt.Settings;

public class JwtSettings : IValidatableObject
{
	/// <summary>
	/// Validate setting
	/// </summary>
	/// <param name="validationContext">The validation context</param>
	/// <returns>A result is a <see cref="ValidationResult"/>.</returns>
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (string.IsNullOrEmpty(Key))
		{
			yield return new ValidationResult("No Key defined in JwtSettings config", new[] { nameof(Key) });
		}
	}

	/// <summary>Get and set key</summary>
	public string Key { get; set; } = string.Empty;
	/// <summary>Get and set token expiration in minutes</summary>
	public int TokenExpirationInMinutes { get; set; }
	/// <summary>Get and set refresh token expiration in days</summary>
	public int RefreshTokenExpirationInDays { get; set; }
}
