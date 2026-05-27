namespace WildRiftCounterLab.Api.Controllers;

using DTOs;
using Services;

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
    public async Task<IActionResult> GetRecommendations([FromBody] DraftRequestDto request)
    {
        var result = await _draftService.GetRecommendations(request);

        return Ok(result);
    }
}