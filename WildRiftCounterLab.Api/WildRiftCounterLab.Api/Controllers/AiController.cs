using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Infrastructure.AI;


namespace WildRiftCounterLab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly GeminiAiExplanationProvider _aiExplanationService;

    public AiController(GeminiAiExplanationProvider aiExplanationService)
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