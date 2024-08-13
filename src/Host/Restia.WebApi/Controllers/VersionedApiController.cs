using Microsoft.AspNetCore.Mvc;

namespace Restia.WebApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public class VersionedApiController : ControllerBase
{
}
