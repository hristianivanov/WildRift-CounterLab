namespace WildRiftCounterLab.Domain.Entities;

public class AiExplanationCache
{
    public int Id { get; set; }

    public string CacheKey { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string LaneEnemy { get; set; } = string.Empty;

    public string EnemyTeamHash { get; set; } = string.Empty;

    public string Champion { get; set; } = string.Empty;

    public string Explanation { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}
