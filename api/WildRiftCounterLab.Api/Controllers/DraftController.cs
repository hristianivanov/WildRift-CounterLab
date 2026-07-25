using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Services;

namespace WildRiftCounterLab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DraftController : ControllerBase
{
    private readonly DraftService _draftService;

    public DraftController(DraftService draftService)
    {
        _draftService = draftService;
    }

    [HttpPost("recommendations")]
    [EnableRateLimiting("draft")]
    [ProducesResponseType(typeof(DraftRecommendationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRecommendations(
        [FromBody] DraftRecommendationRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _draftService.GetRecommendations(request, cancellationToken);

        return Ok(result);
    }
}