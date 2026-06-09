using WildRiftCounterLab.Application.Engine;
using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Tests;

public class ScoreEngineTests
{
    [Fact]
    public void CalculateScore_GivesTankShredValueAgainstTankEnemies()
    {
        var engine = new ScoreEngine();
        var candidate = new Champion
        {
            Name = "Vayne",
            Roles = new List<string> { "Baron" },
            Tags = new List<string> { "marksman", "tank-shred" }
        };
        var tankEnemy = new Champion
        {
            Name = "Dr. Mundo",
            Tags = new List<string> { "tank", "sustain" }
        };
        var neutralEnemy = new Champion
        {
            Name = "Darius",
            Tags = new List<string> { "fighter" }
        };

        var tankScore = engine.CalculateScore(
            candidate,
            "Baron",
            tankEnemy,
            new List<Champion> { tankEnemy },
            new List<MatchupRule>());
        var neutralScore = engine.CalculateScore(
            candidate,
            "Baron",
            neutralEnemy,
            new List<Champion> { neutralEnemy },
            new List<MatchupRule>());

        Assert.True(tankScore.TeamScore > neutralScore.TeamScore);
        Assert.True(tankScore.TotalScore > neutralScore.TotalScore);
    }
}