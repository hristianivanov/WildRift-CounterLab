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
    public IActionResult GetRecommendations([FromBody] DraftRequestDto request)
    {
        var result = _draftService.GetRecommendations(request);

        return Ok(result);
    }
}

public class DraftRequest
{
    public string Role { get; set; } = string.Empty;
    public string LaneEnemy { get; set; } = string.Empty;
    public List<string> EnemyTeam { get; set; } = new();
}