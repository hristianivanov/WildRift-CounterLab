using System.ComponentModel.DataAnnotations;

namespace WildRiftCounterLab.Application.DTOs;

public class CreateMatchupRuleRequestDto
{
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Champion { get; set; } = string.Empty;

    [MaxLength(100)]
    public string EnemyChampion { get; set; } = string.Empty;

    public int ScoreModifier { get; set; }

    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Plan { get; set; }
}