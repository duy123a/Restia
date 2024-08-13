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
	protected ISender Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
