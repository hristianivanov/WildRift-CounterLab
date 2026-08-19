namespace WildRiftCounterLab.Services.Models;

using System.ComponentModel.DataAnnotations;

public class UpdateMatchupTipRequestDto
{
    [Required, MaxLength(100)]
    public string Champion { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string EnemyChampion { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string Tip { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? AbilityTag { get; set; }
}
