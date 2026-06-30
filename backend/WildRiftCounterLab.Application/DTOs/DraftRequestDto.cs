using System.ComponentModel.DataAnnotations;

namespace WildRiftCounterLab.Application.DTOs;

public class DraftRequestDto
{
    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LaneEnemy { get; set; } = string.Empty;

    public List<string> EnemyTeam { get; set; } = new();

    public bool IncludeAiExplanation { get; set; } = false;
}