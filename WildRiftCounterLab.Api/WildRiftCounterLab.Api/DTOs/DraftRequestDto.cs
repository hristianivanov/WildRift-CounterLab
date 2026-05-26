namespace WildRiftCounterLab.Api.DTOs;

public class DraftRequestDto
{
    public string Role { get; set; } = string.Empty;

    public string LaneEnemy { get; set; } = string.Empty;

    public List<string> EnemyTeam { get; set; } = new();
}