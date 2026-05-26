using Microsoft.AspNetCore.Mvc;

namespace WildRiftCounterLab.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DraftController : ControllerBase
{
    [HttpPost("recommendations")]
    public IActionResult GetRecommendations([FromBody] DraftRequest request)
    {
        var result = new[]
        {
            new
            {
                champion = "Malphite",
                score = 92,
                reasons = new[]
                {
                    "Strong into AD champions",
                    "Reliable engage",
                    "Easy execution"
                }
            },
            new
            {
                champion = "Garen",
                score = 86,
                reasons = new[]
                {
                    "Safe lane",
                    "Good sustain",
                    "Simple win condition"
                }
            }
        };

        return Ok(result);
    }
}

public class DraftRequest
{
    public string Role { get; set; } = string.Empty;
    public string LaneEnemy { get; set; } = string.Empty;
    public List<string> EnemyTeam { get; set; } = new();
}