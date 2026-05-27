namespace WildRiftCounterLab.Api.Data;

using Models;


public class ChampionDataProvider
{
    public List<Champion> GetChampions()
    {
        return new List<Champion>
        {
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
            }
        };
    }
}