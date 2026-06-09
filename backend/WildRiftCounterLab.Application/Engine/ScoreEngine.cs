using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Engine;

public class ScoreEngine
{
    private const int BaseScore = 50;

    public ScoreBreakdownDto CalculateScore(
        Champion champion,
        string role,
        Champion laneEnemy,
        IReadOnlyCollection<Champion> enemies,
        IReadOnlyCollection<MatchupRule> rules)
    {
        var championTags = champion.Tags.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var enemyTags = enemies.SelectMany(enemy => enemy.Tags).ToList();

        var laneScore = rules
            .Where(rule =>
                rule.Champion.Equals(champion.Name, StringComparison.OrdinalIgnoreCase) &&
                rule.EnemyChampion.Equals(laneEnemy.Name, StringComparison.OrdinalIgnoreCase))
            .Sum(rule => rule.ScoreModifier);

        if (championTags.Contains("lane-bully") && HasTag(laneEnemy, "scaling"))
        {
            laneScore += 6;
        }

        laneScore = Math.Clamp(laneScore, -30, 30);

        var teamScore = CalculateTeamScore(championTags, enemyTags);
        var roleFitScore = champion.Roles.Contains(role, StringComparer.OrdinalIgnoreCase) ? 8 : 0;
        var safetyScore = CalculateSafetyScore(championTags, enemyTags);
        var scalingScore = CalculateScalingScore(championTags, enemyTags);
        var utilityScore = CalculateUtilityScore(championTags, enemyTags);

        return new ScoreBreakdownDto
        {
            LaneScore = laneScore,
            TeamScore = teamScore,
            RoleFitScore = roleFitScore,
            SafetyScore = safetyScore,
            ScalingScore = scalingScore,
            UtilityScore = utilityScore,
            TotalScore = Math.Clamp(
                BaseScore + laneScore + teamScore + roleFitScore +
                safetyScore + scalingScore + utilityScore,
                0,
                100)
        };
    }

    private static int CalculateTeamScore(HashSet<string> championTags, List<string> enemyTags)
    {
        var score = 0;

        if (championTags.Contains("tank-shred"))
        {
            score += Math.Min(12, CountTags(enemyTags, "tank", "sustain", "juggernaut") * 3);
        }

        if (championTags.Contains("true-damage"))
        {
            score += Math.Min(8, CountTags(enemyTags, "tank", "fighter") * 2);
        }

        if (championTags.Contains("anti-dash"))
        {
            score += Math.Min(12, CountTags(enemyTags, "mobile") * 4);
        }

        if (championTags.Contains("anti-ad"))
        {
            score += Math.Min(10, CountTags(enemyTags, "ad", "marksman", "fighter") * 2);
        }

        return score;
    }

    private static int CalculateSafetyScore(HashSet<string> championTags, List<string> enemyTags)
    {
        var score = 0;

        if (championTags.Contains("safe"))
        {
            score += CountTags(enemyTags, "pick", "burst") > 0 ? 8 : 4;
        }

        if (championTags.Contains("sustain"))
        {
            score += 3;
        }

        return score;
    }

    private static int CalculateScalingScore(HashSet<string> championTags, List<string> enemyTags)
    {
        if (!championTags.Contains("scaling"))
        {
            return 0;
        }

        return CountTags(enemyTags, "scaling") > 0 ? 8 : 4;
    }

    private static int CalculateUtilityScore(HashSet<string> championTags, List<string> enemyTags)
    {
        var score = 0;

        if (championTags.Contains("engage"))
        {
            score += Math.Min(12, CountTags(enemyTags, "marksman", "mage", "immobile") * 3);
        }

        if (championTags.Contains("peel"))
        {
            score += Math.Min(10, CountTags(enemyTags, "assassin", "dive") * 5);
        }

        return score;
    }

    private static int CountTags(IEnumerable<string> tags, params string[] desiredTags)
    {
        return tags.Count(tag => desiredTags.Contains(tag, StringComparer.OrdinalIgnoreCase));
    }

    private static bool HasTag(Champion champion, string tag)
    {
        return champion.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase);
    }
}