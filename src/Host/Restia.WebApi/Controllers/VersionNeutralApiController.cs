using Asp.Versioning;

namespace Restia.WebApi.Controllers;

/// <summary>
/// Version Neutral Api Controller
/// </summary>
[Route("api/[controller]")]
[ApiVersionNeutral]
public class VersionNeutralApiController : BaseApiController
{
}
