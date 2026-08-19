namespace WildRiftCounterLab.Services.Models;

public class MatchupTipDto
{
    public int Id { get; set; }
    public string Champion { get; set; } = string.Empty;
    public string EnemyChampion { get; set; } = string.Empty;
    public string Tip { get; set; } = string.Empty;
    public string? AbilityTag { get; set; }
}
