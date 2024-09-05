using System.Security.Claims;

namespace Restia.Application.Common.Interfaces;

/// <summary>
/// Current User interface
/// </summary>
public interface ICurrentUser
{
	/// <summary>Name</summary>
	string? Name { get; }

	/// <summary>
	/// Get user id
	/// </summary>
	/// <returns>A Guid</returns>
	DefaultIdType GetUserId();

	/// <summary>
	/// Get user email
	/// </summary>
	/// <returns>An Email</returns>
	string? GetUserEmail();

	/// <summary>
	/// Get tenant
	/// </summary>
	/// <returns>A Tenant</returns>
	string? GetTenant();

	/// <summary>
	/// Is authenticated
	/// </summary>
	/// <returns>True: User is authenticated</returns>
	bool IsAuthenticated();

	/// <summary>
	/// Is in role
	/// </summary>
	/// <param name="role">The role</param>
	/// <returns>True: Users have roles</returns>
	bool IsInRole(string role);

	/// <summary>
	/// Get user claims
	/// </summary>
	/// <returns>User claims</returns>
	IEnumerable<Claim>? GetUserClaims();
}
