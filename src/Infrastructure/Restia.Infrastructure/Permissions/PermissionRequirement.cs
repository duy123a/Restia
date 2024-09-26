using Microsoft.AspNetCore.Authorization;

namespace Restia.Infrastructure.Permissions;

internal class PermissionRequirement : IAuthorizationRequirement
{
	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="permission">The permission</param>
	public PermissionRequirement(string permission)
	{
		Permission = permission;
	}

	/// <summary>Get and private set permission</summary>
	public string Permission { get; private set; }
}
