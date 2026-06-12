using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Engine;

public class ScoreEngine
{
    private const int BaseScore = 40;

    public ScoreBreakdownDto CalculateScore(
        Champion champion,
        string role,
        Champion laneEnemy,
        IReadOnlyCollection<Champion> enemies,
        IReadOnlyCollection<MatchupRule> rules)
    {
        var championTags = champion.Tags.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var profile = EnemyDraftProfile.Create(enemies);

        var laneScore = rules
            .Where(rule =>
                rule.Champion.Equals(champion.Name, StringComparison.OrdinalIgnoreCase) &&
                rule.EnemyChampion.Equals(laneEnemy.Name, StringComparison.OrdinalIgnoreCase))
            .Sum(rule => rule.ScoreModifier);

        if (championTags.Contains("lane-bully") && HasTag(laneEnemy, "scaling"))
        {
            laneScore += 7;
        }

        laneScore = Math.Clamp(laneScore, -30, 30);

        var teamScore = CalculateTeamScore(championTags, profile);
        var roleFitScore = champion.Roles.Contains(role, StringComparer.OrdinalIgnoreCase) ? 8 : 0;
        var safetyScore = CalculateSafetyScore(championTags, profile);
        var scalingScore = CalculateScalingScore(championTags, profile);
        var utilityScore = CalculateUtilityScore(championTags, profile);

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

    private static int CalculateTeamScore(HashSet<string> championTags, EnemyDraftProfile profile)
    {
        var score = 0;

        if (championTags.Contains("anti-ad"))
        {
            score += profile.HeavyAd ? 10 : 2;
        }

        if (championTags.Contains("anti-ap"))
        {
            score += profile.HeavyAp ? 10 : 2;
        }

        if (championTags.Contains("tank-shred"))
        {
            score += profile.TankHeavy
                ? 12
                : Math.Min(8, profile.DurableEnemyCount * 2);
        }

        if (championTags.Contains("true-damage"))
        {
            score += profile.TankHeavy
                ? 8
                : Math.Min(6, profile.DurableEnemyCount * 2);
        }

        return Math.Clamp(score, 0, 18);
    }

    private static int CalculateSafetyScore(
        HashSet<string> championTags,
        EnemyDraftProfile profile)
    {
        var score = 0;

        if (championTags.Contains("safe"))
        {
            score += profile.BurstOrPickComp ? 8 : 3;
        }

        if (championTags.Contains("sustain"))
        {
            score += profile.PokeComp ? 7 : 2;
        }

        return Math.Clamp(score, 0, 10);
    }

    private static int CalculateScalingScore(
        HashSet<string> championTags,
        EnemyDraftProfile profile)
    {
        if (!championTags.Contains("scaling"))
        {
            return 0;
        }

        return profile.ScalingComp ? 7 : 3;
    }

    private static int CalculateUtilityScore(
        HashSet<string> championTags,
        EnemyDraftProfile profile)
    {
        var score = 0;

        if (championTags.Contains("engage"))
        {
            score += profile.SquishyBackline || profile.ImmobileCarries ? 8 : 3;
        }

        if (championTags.Contains("peel"))
        {
            score += profile.DiveComp ? 9 : 2;
        }

        if (championTags.Contains("anti-dash"))
        {
            score += profile.MobileComp ? 9 : 2;
        }

        if (championTags.Contains("teamfight"))
        {
            score += profile.GroupedFightComp ? 7 : 3;
        }

        return Math.Clamp(score, 0, 16);
    }

    private static bool HasTag(Champion champion, string tag)
    {
        return champion.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase);
    }
}
