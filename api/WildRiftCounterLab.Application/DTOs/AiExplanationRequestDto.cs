using System.ComponentModel.DataAnnotations;

namespace WildRiftCounterLab.Application.DTOs;

public class AiExplanationRequestDto
{
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty;

    [MaxLength(100)]
    public string LaneEnemy { get; set; } = string.Empty;

    public List<string> EnemyTeam { get; set; } = new();

    [MaxLength(100)]
    public string Champion { get; set; } = string.Empty;

    public int Score { get; set; }

    public List<string> Reasons { get; set; } = new();

    [MaxLength(1000)]
    public string Plan { get; set; } = string.Empty;
}