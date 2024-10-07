using System.Collections.ObjectModel;

namespace Restia.Shared.Authorization;

public static class RestiaRoles
{
	/// <summary>Roles: Admin</summary>
	public const string Admin = nameof(Admin);
	/// <summary>Roles: Basic</summary>
	public const string Basic = nameof(Basic);

	public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(
	[
		Admin,
		Basic
	]);

	/// <summary>
	/// Is default
	/// </summary>
	/// <param name="roleName">The role name</param>
	/// <returns>True: If the role is default</returns>
	public static bool IsDefault(string roleName) => DefaultRoles.Any(r => r == roleName);
}
