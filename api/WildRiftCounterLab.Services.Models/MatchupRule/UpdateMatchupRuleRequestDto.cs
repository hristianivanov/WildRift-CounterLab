namespace WildRiftCounterLab.Services.Models;

using System.ComponentModel.DataAnnotations;

public class UpdateMatchupRuleRequestDto
{
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Champion { get; set; } = string.Empty;

    [MaxLength(100)]
    public string EnemyChampion { get; set; } = string.Empty;

    [Range(-100, 100)]
    public int ScoreModifier { get; set; }

    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Plan { get; set; }
}