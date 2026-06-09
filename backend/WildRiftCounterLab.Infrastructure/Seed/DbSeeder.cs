using WildRiftCounterLab.Domain.Entities;
using WildRiftCounterLab.Infrastructure.Data;


namespace WildRiftCounterLab.Infrastructure.Seed;

public class DbSeeder
{
    public static void Seed(ApplicationDbContext db)
    {
        if (!db.Champions.Any())
        {
            db.Champions.AddRange(
                new Champion
                {
                    Name = "Malphite",
                    Roles = new List<string> { "Baron", "Support" },
                    Tags = new List<string> { "tank", "engage", "anti-ad" }
                },
                new Champion
                {
                    Name = "Garen",
                    Roles = new List<string> { "Baron" },
                    Tags = new List<string> { "fighter", "safe", "sustain" }
                },
                new Champion
                {
                    Name = "Fiora",
                    Roles = new List<string> { "Baron" },
                    Tags = new List<string> { "fighter", "duelist", "true-damage" }
                },
                new Champion
                {
                    Name = "Camille",
                    Roles = new List<string> { "Baron" },
                    Tags = new List<string> { "fighter", "mobile", "true-damage" }
                },
                new Champion
                {
                    Name = "Darius",
                    Roles = new List<string> { "Baron" },
                    Tags = new List<string> { "fighter", "lane-bully", "ad" }
                },
                new Champion
                {
                    Name = "Dr. Mundo",
                    Roles = new List<string> { "Baron", "Jungle" },
                    Tags = new List<string> { "tank", "sustain" }
                },
                new Champion
                {
                    Name = "Vayne",
                    Roles = new List<string> { "Baron", "Dragon" },
                    Tags = new List<string> { "marksman", "tank-shred", "true-damage" }
                },
                new Champion
                {
                    Name = "Olaf",
                    Roles = new List<string> { "Baron", "Jungle" },
                    Tags = new List<string> { "fighter", "lane-bully", "sustain" }
                }
            );
        }

        if (!db.MatchupRules.Any())
        {
            db.MatchupRules.AddRange(
                new MatchupRule
                {
                    Role = "Baron",
                    Champion = "Malphite",
                    EnemyChampion = "Darius",
                    ScoreModifier = 30,
                    Reason = "Armor scaling helps against physical damage",
                    Plan = "Play short trades early. Avoid extended fights before armor items."
                },
                new MatchupRule
                {
                    Role = "Baron",
                    Champion = "Garen",
                    EnemyChampion = "Darius",
                    ScoreModifier = 20,
                    Reason = "Safe lane option with sustain",
                    Plan = "Play short trades early. Avoid extended fights before armor items."
                },
                new MatchupRule
                {
                    Role = "Baron",
                    Champion = "Fiora",
                    EnemyChampion = "Dr. Mundo",
                    ScoreModifier = 35,
                    Reason = "True damage punishes tanky sustain champions",
                    Plan = "Play short trades early. Avoid extended fights before armor items."
                },
                new MatchupRule
                {
                    Role = "Baron",
                    Champion = "Vayne",
                    EnemyChampion = "Dr. Mundo",
                    ScoreModifier = 35,
                    Reason = "Tank shred and range punish Mundo",
                    Plan = "Play short trades early. Avoid extended fights before armor items."
                }
            );

        }

        db.SaveChanges();
    }
}