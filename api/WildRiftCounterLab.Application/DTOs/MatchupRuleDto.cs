namespace WildRiftCounterLab.Application.DTOs;

public class MatchupRuleDto
{
    public int Id { get; set; }

    public string Role { get; set; } = string.Empty;

    public string Champion { get; set; } = string.Empty;

    public string EnemyChampion { get; set; } = string.Empty;

    public int ScoreModifier { get; set; }

    public string Reason { get; set; } = string.Empty;

    public string Plan { get; set; } = string.Empty;
}