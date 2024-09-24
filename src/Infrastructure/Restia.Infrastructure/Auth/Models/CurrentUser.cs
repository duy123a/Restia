using System.Security.Claims;
using Restia.Application.Common.Interfaces;
using Restia.Infrastructure.Auth.Interfaces;
using Restia.Shared.Authorization;

namespace Restia.Infrastructure.Auth.Models;
public class CurrentUser : ICurrentUser, ICurrentUserInitializer
{
	/// <summary>The user</summary>
	private ClaimsPrincipal? _user;
	/// <summary>The user id</summary>
	private DefaultIdType _userId = DefaultIdType.Empty;

	/// <summary>
	/// Get user id
	/// </summary>
	/// <returns>A <see cref="DefaultIdType"/>.</returns>
	public DefaultIdType GetUserId() =>
		IsAuthenticated()
			? DefaultIdType.Parse(_user?.GetUserId() ?? DefaultIdType.Empty.ToString())
			: _userId;

	/// <summary>
	/// Get user email
	/// </summary>
	/// <returns>An Email</returns>
	public string? GetUserEmail() =>
		IsAuthenticated()
			? _user!.GetEmail()
			: string.Empty;

	/// <summary>
	/// Is in role
	/// </summary>
	/// <param name="role">The role</param>
	/// <returns>True: Users have roles</returns>
	public bool IsInRole(string role) =>
		_user?.IsInRole(role) is true;

	/// <summary>
	/// Get user claims
	/// </summary>
	/// <returns>User claims</returns>
	public IEnumerable<Claim>? GetUserClaims() =>
		_user?.Claims;

	/// <summary>
	/// Get tenant
	/// </summary>
	/// <returns>A Tenant</returns>
	public string? GetTenant() =>
		IsAuthenticated() ? _user?.GetTenant() : string.Empty;

	/// <summary>
	/// Set current user
	/// </summary>
	/// <param name="user">The user</param>
	/// <exception cref="Exception">A Exception</exception>
	public void SetCurrentUser(ClaimsPrincipal user)
	{
		if (_user != null)
		{
			throw new Exception("Method reserved for in-scope initialization");
		}

		_user = user;
	}

	/// <summary>
	/// Set current user id
	/// </summary>
	/// <param name="userId">The user id</param>
	public void SetCurrentUserId(string userId)
	{
		if (_userId != DefaultIdType.Empty)
		{
			throw new Exception("Method reserved for in-scope initialization");
		}

		if (!string.IsNullOrEmpty(userId))
		{
			_userId = DefaultIdType.Parse(userId);
		}
	}

	/// <summary>Get user name</summary>
	public string? Name => _user?.Identity?.Name;

	/// <summary>
	/// Is authenticated
	/// </summary>
	/// <returns>True: User is authenticated</returns>
	public bool IsAuthenticated()
	{
		return _user?.Identity?.IsAuthenticated is true;
	}
}
