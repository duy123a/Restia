using Microsoft.AspNetCore.Identity;

namespace Restia.Infrastructure.Identity.Models;

public class ApplicationRole : IdentityRole
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="name">The name</param>
	/// <param name="description">The description</param>
	public ApplicationRole(string name, string? description = null)
		: base(name)
	{
		Description = description;
		NormalizedName = name.ToUpperInvariant();
	}

	/// <summary>Get and set description</summary>
	public string? Description { get; set; }
}
