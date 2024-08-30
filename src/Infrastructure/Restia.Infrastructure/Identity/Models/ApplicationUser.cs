using Microsoft.AspNetCore.Identity;

namespace Restia.Infrastructure.Identity.Models;

public class ApplicationUser : IdentityUser
{
	/// <summary>Get and set first name</summary>
	public string? FirstName { get; set; }
	/// <summary>Get and set last name</summary>
	public string? LastName { get; set; }
	/// <summary>Get and set image url</summary>
	public string? ImageUrl { get; set; }
	/// <summary>Get and set is active</summary>
	public bool IsActive { get; set; }
	/// <summary>Get and set refresh token</summary>
	public string? RefreshToken { get; set; }
	/// <summary>Get and set refresh token expiry time</summary>
	public DateTime RefreshTokenExpiryTime { get; set; }
}
