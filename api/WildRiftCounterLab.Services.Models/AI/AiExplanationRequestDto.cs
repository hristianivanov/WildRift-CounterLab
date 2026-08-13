namespace WildRiftCounterLab.Services.Models;

using System.ComponentModel.DataAnnotations;

public class AiExplanationRequestDto
{
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty;

    [MaxLength(100)]
    public string LaneEnemy { get; set; } = string.Empty;

    [MaxLength(4)]
    public List<string> EnemyTeam { get; set; } = new();

    [MaxLength(100)]
    public string Champion { get; set; } = string.Empty;

    [Range(0, 100)]
    public int Score { get; set; }

    [MaxLength(20)]
    public List<string> Reasons { get; set; } = new();

    [MaxLength(1000)]
    public string Plan { get; set; } = string.Empty;
}