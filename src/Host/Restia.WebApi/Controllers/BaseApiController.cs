using MediatR;

namespace Restia.WebApi.Controllers;

/// <summary>
/// Base api controller
/// </summary>
[ApiController]
public class BaseApiController : ControllerBase
{
	/// <summary>The mediator</summary>
	private ISender _mediator = null!;

	/// <summary>Get mediator</summary>
	/// This is the same as you get from contructor, using a lazy-loaded property which fetches ISender from the service provider when first accessed
	/// This is a service locator pattern
	protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
