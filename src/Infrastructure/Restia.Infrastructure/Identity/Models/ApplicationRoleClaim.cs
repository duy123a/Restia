using Microsoft.AspNetCore.Identity;

namespace Restia.Infrastructure.Identity.Models;

public class ApplicationRoleClaim : IdentityRoleClaim<string>
{
	/// <summary>Get and set created by</summary>
	public string? CreatedBy { get; init; }
	/// <summary>Get and set created on</summary>
	public DateTime CreatedOn { get; init; }
}
