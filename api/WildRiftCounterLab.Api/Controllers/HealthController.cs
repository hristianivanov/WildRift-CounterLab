using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WildRiftCounterLab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    public IActionResult GetHealth()
    {
        return Ok(new HealthResponse("ok"));
    }

    public sealed record HealthResponse(string Status);
}
