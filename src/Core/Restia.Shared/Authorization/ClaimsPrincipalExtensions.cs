using System.Security.Claims;

namespace Restia.Shared.Authorization;

public static class ClaimsPrincipalExtensions
{
	/// <summary>
	/// Get email
	/// </summary>
	/// <param name="principal">The principal</param>
	/// <returns>A email</returns>
	public static string? GetEmail(this ClaimsPrincipal principal)
		=> principal.FindFirstValue(ClaimTypes.Email);

	/// <summary>
	/// Get Tenant
	/// </summary>
	/// <param name="principal">The principal</param>
	/// <returns>A tenant</returns>
	public static string? GetTenant(this ClaimsPrincipal principal)
		=> principal.FindFirstValue(RestiaClaims.Tenant);

	/// <summary>
	/// Get fullname
	/// </summary>
	/// <param name="principal">The principal</param>
	/// <returns>A fullname</returns>
	public static string? GetFullName(this ClaimsPrincipal principal)
		=> principal?.FindFirst(RestiaClaims.Fullname)?.Value;

	/// <summary>
	/// Get first name
	/// </summary>
	/// <param name="principal">The principal</param>
	/// <returns>A first name</returns>
	public static string? GetFirstName(this ClaimsPrincipal principal)
		=> principal?.FindFirst(ClaimTypes.Name)?.Value;

	/// <summary>
	/// Get surname
	/// </summary>
	/// <param name="principal">The principal</param>
	/// <returns>A surname</returns>
	public static string? GetSurname(this ClaimsPrincipal principal)
		=> principal?.FindFirst(ClaimTypes.Surname)?.Value;

	/// <summary>
	/// Get phone number
	/// </summary>
	/// <param name="principal">The principal</param>
	/// <returns>A phone number</returns>
	public static string? GetPhoneNumber(this ClaimsPrincipal principal)
		=> principal.FindFirstValue(ClaimTypes.MobilePhone);

	/// <summary>
	/// Get user id
	/// </summary>
	/// <param name="principal">The principal</param>
	/// <returns>A user id</returns>
	public static string? GetUserId(this ClaimsPrincipal principal)
		=> principal.FindFirstValue(ClaimTypes.NameIdentifier);

	/// <summary>
	/// Get image url
	/// </summary>
	/// <param name="principal">The principal</param>
	/// <returns>A image url</returns>
	public static string? GetImageUrl(this ClaimsPrincipal principal)
		=> principal.FindFirstValue(RestiaClaims.ImageUrl);

	/// <summary>
	/// Get expiration
	/// </summary>
	/// <param name="principal">The principal</param>
	/// <returns>A expiration time</returns>
	public static DateTimeOffset GetExpiration(this ClaimsPrincipal principal)
		=> DateTimeOffset.FromUnixTimeSeconds(
			Convert.ToInt64(principal.FindFirstValue(RestiaClaims.Expiration)));

	/// <summary>
	/// Find first value
	/// </summary>
	/// <param name="principal">The principal</param>
	/// <param name="claimType">The claim type</param>
	/// <returns>A first value</returns>
	/// <exception cref="ArgumentNullException">A ArgumentNullException</exception>
	private static string? FindFirstValue(this ClaimsPrincipal principal, string claimType)
	{
		return principal is null
			? throw new ArgumentNullException(nameof(principal))
			: principal.FindFirst(claimType)?.Value;
	}
}
