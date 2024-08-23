using System.ComponentModel.DataAnnotations;

namespace Restia.Infrastructure.Persistence.Settings;

/// <summary>
/// Database Settings
/// </summary>
public class DatabaseSettings : IValidatableObject
{
	/// <summary>
	/// Validate setting
	/// </summary>
	/// <param name="validationContext">The validation context</param>
	/// <returns>A result is a <see cref="ValidationResult"/>.</returns>
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (string.IsNullOrEmpty(DBProvider))
		{
			yield return new ValidationResult(
				$"{nameof(DatabaseSettings)}.{nameof(DBProvider)} is not configured",
				new[] { nameof(DBProvider) });
		}

		if (string.IsNullOrEmpty(ConnectionString))
		{
			yield return new ValidationResult(
				$"{nameof(DatabaseSettings)}.{nameof(ConnectionString)} is not configured",
				new[] { nameof(ConnectionString) });
		}
	}

	/// <summary>DB Provider</summary>
	public string DBProvider { get; set; } = string.Empty;
	/// <summary>Connection string</summary>
	public string ConnectionString { get; set; } = string.Empty;
}
