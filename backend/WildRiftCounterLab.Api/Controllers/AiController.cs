using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    [ProducesResponseType(typeof(AiExplanationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Explain([FromBody] AiExplanationRequestDto request)
    {
        var result = await _aiExplanationProvider.ExplainAsync(request);

        return Ok(result);
    }
}