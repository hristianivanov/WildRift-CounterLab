using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Interfaces;

namespace WildRiftCounterLab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly IAiExplanationProvider _aiExplanationProvider;

    public AiController(IAiExplanationProvider aiExplanationProvider)
    {
        _aiExplanationProvider = aiExplanationProvider;
    }

    [HttpPost("explain")]
    [EnableRateLimiting("ai")]
    [ProducesResponseType(typeof(AiExplanationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Explain(
        [FromBody] AiExplanationRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _aiExplanationProvider.ExplainAsync(request, cancellationToken);

        return Ok(result);
    }
}