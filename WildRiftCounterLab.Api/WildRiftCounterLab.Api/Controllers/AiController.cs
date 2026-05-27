using Microsoft.AspNetCore.Mvc;

using WildRiftCounterLab.Api.DTOs;
using WildRiftCounterLab.Api.Services;

namespace WildRiftCounterLab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly AiExplanationService _aiExplanationService;

    public AiController(AiExplanationService aiExplanationService)
    {
        _aiExplanationService = aiExplanationService;
    }

    [HttpPost("explain")]
    public async Task<IActionResult> Explain([FromBody] AiExplanationRequestDto request)
    {
        var result = await _aiExplanationService.ExplainAsync(request);

        return Ok(result);
    }
}