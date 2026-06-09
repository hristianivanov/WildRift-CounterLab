namespace WildRiftCounterLab.Application.DTOs;

public class DraftRecommendationRequestDto
{
    public string Role { get; set; } = string.Empty;

    public string LaneEnemy { get; set; } = string.Empty;

    public List<string> EnemyTeam { get; set; } = new();

    public bool IncludeAiExplanation { get; set; } = false;
}