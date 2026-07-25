using Microsoft.EntityFrameworkCore;

using WildRiftCounterLab.Domain.Entities;
using WildRiftCounterLab.Infrastructure.Data;
using WildRiftCounterLab.Infrastructure.Seed;

namespace WildRiftCounterLab.Api.Tests;

public class DbSeederTests
{
    [Fact]
    public void Seed_IsIdempotentAndAddsMissingChampionsWithoutChangingExistingData()
    {
        using var db = CreateDbContext();
        db.Champions.Add(new Champion
        {
            Name = "Malphite",
            Roles = new List<string> { "Baron" },
            Tags = new List<string> { "custom-tag" }
        });
        db.SaveChanges();

        DbSeeder.Seed(db);
        var championCountAfterFirstSeed = db.Champions.Count();
        var ruleCountAfterFirstSeed = db.MatchupRules.Count();

        DbSeeder.Seed(db);

        Assert.Equal(championCountAfterFirstSeed, db.Champions.Count());
        Assert.Equal(ruleCountAfterFirstSeed, db.MatchupRules.Count());
        Assert.Equal(
            db.Champions.Count(),
            db.Champions.Select(champion => champion.Name.ToUpper()).Distinct().Count());

        var malphite = Assert.Single(db.Champions, champion => champion.Name == "Malphite");
        Assert.Equal(new List<string> { "custom-tag" }, malphite.Tags);
        Assert.Contains(db.Champions, champion => champion.Name == "Senna");
        Assert.Contains(db.Champions, champion => champion.Name == "Jhin");
    }

    private static ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"DbSeederTests-{Guid.NewGuid()}")
            .Options;

        return new ApplicationDbContext(options);
    }
}
