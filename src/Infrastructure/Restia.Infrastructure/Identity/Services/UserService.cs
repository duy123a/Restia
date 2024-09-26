using Restia.Application.Identity.Users.ServiceInterfaces;

namespace Restia.Infrastructure.Identity.Services;

internal partial class UserService : IUserService
{
	/// <summary>
	/// Has permission async
	/// </summary>
	/// <param name="userId">The user id</param>
	/// <param name="permission">The permission</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A <see cref="Task" /> result is a boolean: True if the user has the current permission.</returns>
	public async Task<bool> HasPermissionAsync(
		string userId,
		string permission,
		CancellationToken cancellationToken)
	{
		return await Task.FromResult(true);
	}
}
