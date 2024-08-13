namespace Restia.WebApi.Controllers;

/// <summary>
/// Versioned api controller
/// </summary>
[Route("api/v{version:apiVersion}/[controller]")]
public class VersionedApiController : BaseApiController
{
}
