using System.Security.Claims;

namespace Restia.Infrastructure.Auth.Interfaces;

public interface ICurrentUserInitializer
{
	/// <summary>
	/// Set current user
	/// </summary>
	/// <param name="user">The user</param>
	void SetCurrentUser(ClaimsPrincipal user);

	/// <summary>
	/// Set current user id
	/// </summary>
	/// <param name="userId">The user id</param>
	void SetCurrentUserId(string userId);
}
