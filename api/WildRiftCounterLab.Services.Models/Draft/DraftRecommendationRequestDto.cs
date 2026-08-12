namespace WildRiftCounterLab.Services.Models;

public class DraftRecommendationRequestDto
{
    public string Role { get; set; } = string.Empty;

    public string? LaneEnemy { get; set; }

    public List<string> EnemyTeam { get; set; } = new();

    public bool IncludeAiExplanation { get; set; } = false;
}