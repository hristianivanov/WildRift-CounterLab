namespace WildRiftCounterLab.Api.DTOs;

public class AiExplanationRequestDto
{
    public string Role { get; set; } = string.Empty;

    public string LaneEnemy { get; set; } = string.Empty;

    public List<string> EnemyTeam { get; set; } = new();

    public string Champion { get; set; } = string.Empty;

    public int Score { get; set; }

    public List<string> Reasons { get; set; } = new();

    public string Plan { get; set; } = string.Empty;
}