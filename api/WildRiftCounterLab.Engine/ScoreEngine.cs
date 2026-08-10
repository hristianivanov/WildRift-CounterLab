namespace WildRiftCounterLab.Engine;

using WildRiftCounterLab.Data.Models;
using WildRiftCounterLab.Services.Models;

public class ScoreEngine
{
    private const int BaseScore = 40;
    private const int LaneScoreClamp = 30;
    private const int LaneBullyVsScalingBonus = 7;
    private const int RoleFitBonus = 8;
    private const int AntiDamageOnTheme = 10;
    private const int AntiDamageOffTheme = 2;
    private const int TankShredOnTheme = 12;
    private const int TankShredOffTheme = 8;
    private const int TrueDamageOnTheme = 8;
    private const int TrueDamageOffTheme = 6;
    private const int TeamScoreCap = 18;
    private const int SafeVsBurstPick = 8;
    private const int SafeOffTheme = 3;
    private const int SustainVsPoke = 7;
    private const int SustainOffTheme = 2;
    private const int SafetyScoreCap = 10;
    private const int ScalingWithScalingComp = 7;
    private const int ScalingAgainstComp = 3;
    private const int EngageVsSquishyOrImmobile = 8;
    private const int EngageOffTheme = 3;
    private const int PeelVsDive = 9;
    private const int PeelOffTheme = 2;
    private const int AntiDashVsMobileComp = 9;
    private const int AntiDashOffTheme = 2;
    private const int TeamfightInGroupedComp = 7;
    private const int TeamfightOffTheme = 3;
    private const int UtilityScoreCap = 16;

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
            laneScore += LaneBullyVsScalingBonus;
        }

        laneScore = Math.Clamp(laneScore, -LaneScoreClamp, LaneScoreClamp);

        var teamScore = CalculateTeamScore(championTags, profile);
        var roleFitScore = champion.Roles.Contains(role, StringComparer.OrdinalIgnoreCase) ? RoleFitBonus : 0;
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
            score += profile.HeavyAd ? AntiDamageOnTheme : AntiDamageOffTheme;

        if (championTags.Contains("anti-ap"))
            score += profile.HeavyAp ? AntiDamageOnTheme : AntiDamageOffTheme;

        if (championTags.Contains("tank-shred"))
            score += profile.TankHeavy ? TankShredOnTheme : Math.Min(TankShredOffTheme, profile.DurableEnemyCount * 2);

        if (championTags.Contains("true-damage"))
            score += profile.TankHeavy ? TrueDamageOnTheme : Math.Min(TrueDamageOffTheme, profile.DurableEnemyCount * 2);

        return Math.Clamp(score, 0, TeamScoreCap);
    }

    private static int CalculateSafetyScore(HashSet<string> championTags, EnemyDraftProfile profile)
    {
        var score = 0;

        if (championTags.Contains("safe"))
            score += profile.BurstOrPickComp ? SafeVsBurstPick : SafeOffTheme;

        if (championTags.Contains("sustain"))
            score += profile.PokeComp ? SustainVsPoke : SustainOffTheme;

        return Math.Clamp(score, 0, SafetyScoreCap);
    }

    private static int CalculateScalingScore(HashSet<string> championTags, EnemyDraftProfile profile)
    {
        if (!championTags.Contains("scaling"))
            return 0;

        return profile.ScalingComp ? ScalingWithScalingComp : ScalingAgainstComp;
    }

    private static int CalculateUtilityScore(HashSet<string> championTags, EnemyDraftProfile profile)
    {
        var score = 0;

        if (championTags.Contains("engage"))
            score += profile.SquishyBackline || profile.ImmobileCarries ? EngageVsSquishyOrImmobile : EngageOffTheme;

        if (championTags.Contains("peel"))
            score += profile.DiveComp ? PeelVsDive : PeelOffTheme;

        if (championTags.Contains("anti-dash"))
            score += profile.MobileComp ? AntiDashVsMobileComp : AntiDashOffTheme;

        if (championTags.Contains("teamfight"))
            score += profile.GroupedFightComp ? TeamfightInGroupedComp : TeamfightOffTheme;

        return Math.Clamp(score, 0, UtilityScoreCap);
    }

    private static bool HasTag(Champion champion, string tag)
        => champion.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase);
}