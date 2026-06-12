using WildRiftCounterLab.Application.Engine;
using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Tests;

public class ScoreEngineTests
{
    private readonly ScoreEngine _engine = new();

    [Fact]
    public void CalculateScore_GivesAntiAdValueIntoHeavyAd()
    {
        var candidate = Champion("Malphite", "anti-ad");
        var heavyAd = Enemies(
            Champion("Jhin", "marksman", "ad"),
            Champion("Darius", "fighter", "ad"),
            Champion("Olaf", "fighter", "ad"));
        var mixed = Enemies(
            Champion("Ahri", "mage", "ap"),
            Champion("Darius", "fighter", "ad"),
            Champion("Nami", "support"));

        Assert.True(Score(candidate, heavyAd).TeamScore > Score(candidate, mixed).TeamScore);
    }

    [Fact]
    public void CalculateScore_GivesTankShredValueIntoTankHeavyDraft()
    {
        var candidate = Champion("Vayne", "tank-shred");
        var tankHeavy = Enemies(
            Champion("Ornn", "tank"),
            Champion("Rammus", "tank"),
            Champion("Darius", "fighter"));
        var squishy = Enemies(
            Champion("Jhin", "marksman"),
            Champion("Lux", "mage"),
            Champion("Senna", "marksman"));

        Assert.True(Score(candidate, tankHeavy).TeamScore > Score(candidate, squishy).TeamScore);
    }

    [Fact]
    public void CalculateScore_GivesEngageValueIntoImmobileBackline()
    {
        var candidate = Champion("Malphite", "engage");
        var immobileBackline = Enemies(
            Champion("Jhin", "marksman", "immobile"),
            Champion("Lux", "mage", "immobile"),
            Champion("Nami", "support"));
        var durableDraft = Enemies(
            Champion("Ornn", "tank"),
            Champion("Darius", "fighter"),
            Champion("Olaf", "fighter"));

        Assert.True(
            Score(candidate, immobileBackline).UtilityScore >
            Score(candidate, durableDraft).UtilityScore);
    }

    [Fact]
    public void CalculateScore_GivesPeelValueIntoDiveAndAssassins()
    {
        var candidate = Champion("Janna", "peel");
        var diveDraft = Enemies(
            Champion("Zed", "assassin", "dive"),
            Champion("Akali", "assassin", "dive"),
            Champion("Jhin", "marksman"));
        var pokeDraft = Enemies(
            Champion("Lux", "mage", "poke"),
            Champion("Varus", "marksman", "poke"),
            Champion("Nami", "support"));

        Assert.True(Score(candidate, diveDraft).UtilityScore > Score(candidate, pokeDraft).UtilityScore);
    }

    [Fact]
    public void CalculateScore_GivesAntiDashValueIntoMobileDraft()
    {
        var candidate = Champion("Vex", "anti-dash");
        var mobileDraft = Enemies(
            Champion("Akali", "mobile"),
            Champion("Lee Sin", "mobile"),
            Champion("Jhin", "immobile"));
        var immobileDraft = Enemies(
            Champion("Jhin", "immobile"),
            Champion("Lux", "immobile"),
            Champion("Nami", "support"));

        Assert.True(
            Score(candidate, mobileDraft).UtilityScore >
            Score(candidate, immobileDraft).UtilityScore);
    }

    [Fact]
    public void CalculateScore_GivesLaneBullyValueIntoScalingLane()
    {
        var candidate = Champion("Renekton", "lane-bully");
        var scalingEnemy = Champion("Nasus", "scaling");
        var neutralEnemy = Champion("Darius", "fighter");

        var scalingScore = _engine.CalculateScore(
            candidate,
            "Baron",
            scalingEnemy,
            Enemies(scalingEnemy),
            Array.Empty<MatchupRule>());
        var neutralScore = _engine.CalculateScore(
            candidate,
            "Baron",
            neutralEnemy,
            Enemies(neutralEnemy),
            Array.Empty<MatchupRule>());

        Assert.True(scalingScore.LaneScore > neutralScore.LaneScore);
    }

    [Fact]
    public void BuildReasons_IncludesProfileBasedExplanationAndCapsAtFour()
    {
        var reasons = new ReasonEngine().BuildReasons(
            Champion("Malphite", "anti-ad", "engage", "teamfight"),
            Enemies(
                Champion("Jhin", "marksman", "ad", "immobile"),
                Champion("Darius", "fighter", "ad"),
                Champion("Olaf", "fighter", "ad")),
            Array.Empty<MatchupRule>());

        Assert.Contains(reasons, reason => reason.Contains("heavy physical damage"));
        Assert.True(reasons.Count <= 4);
        Assert.Equal(reasons.Count, reasons.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    [Fact]
    public void BuildPlan_UsesProfileFallbackWhenNoMatchupPlanExists()
    {
        var plan = new PlanEngine().BuildPlan(
            Champion("Garen", "fighter"),
            Array.Empty<MatchupRule>(),
            Enemies(
                Champion("Lux", "poke"),
                Champion("Varus", "poke"),
                Champion("Nami", "support")));

        Assert.Contains("enemy poke", plan);
    }

    [Fact]
    public void BuildPlan_UsesMatchupRulePlanBeforeProfileFallback()
    {
        var plan = new PlanEngine().BuildPlan(
            Champion("Malphite", "engage"),
            new[]
            {
                new MatchupRule
                {
                    Champion = "Malphite",
                    Plan = "Play short trades early."
                }
            },
            Enemies(Champion("Lux", "poke"), Champion("Varus", "poke")));

        Assert.Equal("Play short trades early.", plan);
    }

    [Fact]
    public void CalculateScore_ClampsTotalAndKeepsBackwardCompatibleBreakdown()
    {
        var candidate = Champion(
            "Perfect Pick",
            "anti-ad",
            "tank-shred",
            "true-damage",
            "engage",
            "peel",
            "anti-dash",
            "safe",
            "sustain",
            "scaling",
            "teamfight");
        var enemies = Enemies(
            Champion("One", "tank", "ad", "mobile", "dive", "poke", "scaling"),
            Champion("Two", "tank", "ad", "mobile", "assassin", "poke", "scaling"),
            Champion("Three", "fighter", "ad", "burst", "pick"));

        var breakdown = Score(candidate, enemies);

        Assert.InRange(breakdown.TotalScore, 0, 100);
        Assert.Equal(
            breakdown.TotalScore,
            Math.Clamp(
                40 + breakdown.LaneScore + breakdown.TeamScore + breakdown.RoleFitScore +
                breakdown.SafetyScore + breakdown.ScalingScore + breakdown.UtilityScore,
                0,
                100));
    }

    [Fact]
    public void CalculateScore_StrongRealisticPickDoesNotAutomaticallyReachOneHundred()
    {
        var candidate = Champion("Malphite", "anti-ad", "engage");
        var enemies = Enemies(
            Champion("Jhin", "marksman", "ad", "immobile"),
            Champion("Darius", "fighter", "ad"),
            Champion("Olaf", "fighter", "ad"));
        var rules = new[]
        {
            new MatchupRule
            {
                Champion = "Malphite",
                EnemyChampion = "Jhin",
                ScoreModifier = 30
            }
        };

        var breakdown = _engine.CalculateScore(candidate, "Baron", enemies.First(), enemies, rules);

        Assert.InRange(breakdown.TotalScore, 80, 99);
    }

    [Fact]
    public void EnemyDraftProfile_DetectsBroaderCompositionSignals()
    {
        var profile = EnemyDraftProfile.Create(Enemies(
            Champion("Ahri", "mage", "ap", "mobile", "burst", "scaling"),
            Champion("Akali", "assassin", "ap", "mobile", "dive", "scaling"),
            Champion("Lux", "mage", "ap", "immobile", "poke", "pick")));

        Assert.True(profile.HeavyAp);
        Assert.True(profile.SquishyBackline);
        Assert.True(profile.MobileComp);
        Assert.True(profile.ScalingComp);
        Assert.True(profile.LowFrontlineComp);
        Assert.True(profile.ImmobileCarries);
        Assert.True(profile.BurstOrPickComp);
    }

    private WildRiftCounterLab.Application.DTOs.ScoreBreakdownDto Score(
        Champion candidate,
        IReadOnlyCollection<Champion> enemies)
    {
        return _engine.CalculateScore(
            candidate,
            "Baron",
            enemies.First(),
            enemies,
            Array.Empty<MatchupRule>());
    }

    private static Champion Champion(string name, params string[] tags)
    {
        return new Champion
        {
            Name = name,
            Roles = new List<string> { "Baron" },
            Tags = tags.ToList()
        };
    }

    private static IReadOnlyCollection<Champion> Enemies(params Champion[] champions)
    {
        return champions;
    }
}
