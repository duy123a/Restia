using Microsoft.AspNetCore.Http;
using Restia.Infrastructure.Auth.Interfaces;

namespace Restia.Infrastructure.Auth.Middleware;

public class CurrentUserMiddleware : IMiddleware
{
	/// <summary>The current user initializer</summary>
	private readonly ICurrentUserInitializer _currentUserInitializer;

	/// <summary>
	/// Constructor
	/// </summary>
	/// <param name="currentUserInitializer">The current user initializer</param>
	public CurrentUserMiddleware(ICurrentUserInitializer currentUserInitializer) =>
		_currentUserInitializer = currentUserInitializer;

	/// <summary>
	/// Invoke async
	/// </summary>
	/// <param name="context">The http context</param>
	/// <param name="next">The next handler</param>
	/// <returns>A task</returns>
	public async Task InvokeAsync(HttpContext context, RequestDelegate next)
	{
		_currentUserInitializer.SetCurrentUser(context.User);

		await next(context);
	}
}
