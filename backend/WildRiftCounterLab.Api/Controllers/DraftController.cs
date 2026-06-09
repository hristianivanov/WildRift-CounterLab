using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Services;


namespace WildRiftCounterLab.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

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
    [ProducesResponseType(typeof(DraftRecommendationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetRecommendations([FromBody] DraftRecommendationRequestDto request)
    {
        var result = await _draftService.GetRecommendations(request);

        return Ok(result);
    }
}