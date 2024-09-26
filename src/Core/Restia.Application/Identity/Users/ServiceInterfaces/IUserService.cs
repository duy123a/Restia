using Restia.Application.Common.Interfaces;

namespace Restia.Application.Identity.Users.ServiceInterfaces;
public interface IUserService : ITransientService
{
	/// <summary>
	/// Has permission async
	/// </summary>
	/// <param name="userId">The user id</param>
	/// <param name="permission">The permission</param>
	/// <param name="cancellationToken">The cancellation token</param>
	/// <returns>A <see cref="Task" /> result is a boolean: True if the user has the current permission.</returns>
	Task<bool> HasPermissionAsync(
		string userId,
		string permission,
		CancellationToken cancellationToken = default);
}
