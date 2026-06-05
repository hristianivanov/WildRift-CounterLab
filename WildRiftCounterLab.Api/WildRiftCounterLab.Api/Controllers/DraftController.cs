using System.Threading.Tasks;

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
    public async Task<IActionResult> GetRecommendations([FromBody] DraftRecommendationRequestDto request)
    {
        var result = await _draftService.GetRecommendations(request);

        return Ok(result);
    }
}