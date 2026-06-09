using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Engine;

public class ReasonEngine
{
    public List<string> BuildReasons(
        Champion champion,
        IReadOnlyCollection<Champion> enemies,
        IReadOnlyCollection<MatchupRule> rules)
    {
        var reasons = rules
            .Where(rule => rule.Champion.Equals(champion.Name, StringComparison.OrdinalIgnoreCase))
            .Select(rule => rule.Reason)
            .Where(reason => !string.IsNullOrWhiteSpace(reason))
            .ToList();

        var championTags = champion.Tags.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var enemyTags = enemies
            .SelectMany(enemy => enemy.Tags)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        AddTagReason(reasons, championTags, enemyTags, "tank-shred", "tank",
            "Tank shred is valuable into durable enemy champions.");
        AddTagReason(reasons, championTags, enemyTags, "true-damage", "fighter",
            "True damage stays effective against durable fighters.");
        AddTagReason(reasons, championTags, enemyTags, "engage", "marksman",
            "Reliable engage is valuable into enemy carries.");
        AddTagReason(reasons, championTags, enemyTags, "peel", "assassin",
            "Peel helps protect allies from enemy assassins.");
        AddTagReason(reasons, championTags, enemyTags, "anti-dash", "mobile",
            "Anti-dash tools punish mobile enemy champions.");
        AddTagReason(reasons, championTags, enemyTags, "safe", "burst",
            "Safe laning reduces the risk from burst-heavy drafts.");
        AddTagReason(reasons, championTags, enemyTags, "anti-ad", "ad",
            "Tank profile fits heavy physical damage enemy drafts.");

        if (championTags.Contains("scaling") && enemyTags.Contains("scaling"))
        {
            reasons.Add("Strong scaling keeps pace with the enemy late game.");
        }

        if (reasons.Count == 0)
        {
            reasons.Add("Solid role fit with a dependable general game plan.");
        }

        return reasons
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(4)
            .ToList();
    }

    private static void AddTagReason(
        List<string> reasons,
        HashSet<string> championTags,
        HashSet<string> enemyTags,
        string championTag,
        string enemyTag,
        string reason)
    {
        if (championTags.Contains(championTag) && enemyTags.Contains(enemyTag))
        {
            reasons.Add(reason);
        }
    }
}