using System.ComponentModel.DataAnnotations;


namespace WildRiftCounterLab.Application.DTOs;

public class DraftRequestDto
{
    [Required]
    public string Role { get; set; } = string.Empty;

    [Required]
    public string LaneEnemy { get; set; } = string.Empty;

    public List<string> EnemyTeam { get; set; } = new();

    public bool IncludeAiExplanation { get; set; } = false;

}