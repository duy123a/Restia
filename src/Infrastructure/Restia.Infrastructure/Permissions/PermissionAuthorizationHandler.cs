using Microsoft.AspNetCore.Authorization;
using Restia.Application.Identity.Users.ServiceInterfaces;
using Restia.Shared.Authorization;

namespace Restia.Infrastructure.Permissions;

internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
	/// <summary>The user service</summary>
	private readonly IUserService _userService;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="userService">The user service</param>
	public PermissionAuthorizationHandler(IUserService userService) =>
		_userService = userService;

	/// <summary>
	/// Handle requirement async
	/// </summary>
	/// <param name="context">The context</param>
	/// <param name="requirement">The requirement</param>
	/// <returns>A task</returns>
	protected override async Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		PermissionRequirement requirement)
	{
		// { } mean non-null object, which if not null will assign the value to userId
		if (context.User?.GetUserId() is { } userId
			&& await _userService.HasPermissionAsync(userId, requirement.Permission))
		{
			context.Succeed(requirement);
		}
	}
}
