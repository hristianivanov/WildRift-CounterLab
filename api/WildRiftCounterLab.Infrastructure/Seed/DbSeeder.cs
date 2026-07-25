using WildRiftCounterLab.Domain.Entities;
using WildRiftCounterLab.Infrastructure.Data;

namespace WildRiftCounterLab.Infrastructure.Seed;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext db)
    {
        AddMissingChampions(db);
        AddMissingMatchupRules(db);
        db.SaveChanges();
    }

    private static void AddMissingChampions(ApplicationDbContext db)
    {
        var existingNames = db.Champions
            .Select(champion => champion.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missingChampions = InitialChampions()
            .Where(champion => existingNames.Add(champion.Name))
            .ToList();

        if (missingChampions.Count > 0)
        {
            db.Champions.AddRange(missingChampions);
        }
    }

    private static void AddMissingMatchupRules(ApplicationDbContext db)
    {
        var existingKeys = db.MatchupRules
            .AsEnumerable()
            .Select(RuleKey)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missingRules = InitialMatchupRules()
            .Where(rule => existingKeys.Add(RuleKey(rule)))
            .ToList();

        if (missingRules.Count > 0)
        {
            db.MatchupRules.AddRange(missingRules);
        }
    }

    private static string RuleKey(MatchupRule rule)
    {
        return $"{rule.Role}|{rule.Champion}|{rule.EnemyChampion}";
    }

    private static IReadOnlyCollection<Champion> InitialChampions()
    {
        return new List<Champion>
        {
            Champion("Malphite", ["Baron", "Support"], ["tank", "engage", "anti-ad"]),
            Champion("Garen", ["Baron"], ["fighter", "safe", "sustain", "ad"]),
            Champion("Fiora", ["Baron"], ["fighter", "mobile", "true-damage"]),
            Champion("Camille", ["Baron"], ["fighter", "mobile", "true-damage", "dive"]),
            Champion("Darius", ["Baron"], ["fighter", "lane-bully", "ad"]),
            Champion("Dr. Mundo", ["Baron", "Jungle"], ["tank", "sustain", "scaling"]),
            Champion("Vayne", ["Baron", "Dragon"], ["marksman", "tank-shred", "true-damage", "scaling"]),
            Champion("Olaf", ["Baron", "Jungle"], ["fighter", "lane-bully", "sustain", "ad"]),
            Champion("Renekton", ["Baron"], ["fighter", "lane-bully", "dive", "ad"]),
            Champion("Sett", ["Baron"], ["fighter", "sustain", "engage", "ad"]),
            Champion("Nasus", ["Baron"], ["fighter", "sustain", "scaling"]),
            Champion("Gwen", ["Baron"], ["fighter", "scaling", "true-damage", "mobile"]),
            Champion("Jax", ["Baron", "Jungle"], ["fighter", "scaling", "dive", "mobile"]),
            Champion("Teemo", ["Baron"], ["mage", "poke", "safe"]),
            Champion("Ornn", ["Baron"], ["tank", "engage", "scaling", "cc"]),
            Champion("Shen", ["Baron"], ["tank", "peel", "safe", "cc"]),

            Champion("Lee Sin", ["Jungle"], ["fighter", "mobile", "dive", "jungle"]),
            Champion("Vi", ["Jungle"], ["fighter", "engage", "dive", "cc"]),
            Champion("Wukong", ["Jungle", "Baron"], ["fighter", "engage", "dive", "cc"]),
            Champion("Kha'Zix", ["Jungle"], ["assassin", "burst", "mobile", "jungle"]),
            Champion("Master Yi", ["Jungle"], ["assassin", "scaling", "mobile", "dive"]),
            Champion("Rammus", ["Jungle"], ["tank", "anti-ad", "engage", "cc"]),
            Champion("Amumu", ["Jungle"], ["tank", "engage", "cc", "mage"]),
            Champion("Gragas", ["Jungle", "Mid", "Baron"], ["tank", "engage", "peel", "burst"]),
            Champion("Xin Zhao", ["Jungle"], ["fighter", "engage", "dive", "sustain"]),
            Champion("Nunu & Willump", ["Jungle"], ["tank", "engage", "sustain", "cc"]),
            Champion("Shyvana", ["Jungle"], ["fighter", "scaling", "dive", "jungle"]),
            Champion("Warwick", ["Jungle", "Baron"], ["fighter", "sustain", "engage", "jungle"]),

            Champion("Ahri", ["Mid"], ["mage", "mobile", "burst", "cc"]),
            Champion("Yasuo", ["Mid", "Baron"], ["fighter", "mobile", "scaling", "ad"]),
            Champion("Zed", ["Mid"], ["assassin", "burst", "mobile", "ad"]),
            Champion("Lux", ["Mid", "Support"], ["mage", "poke", "burst", "immobile"]),
            Champion("Katarina", ["Mid"], ["assassin", "burst", "mobile", "dive"]),
            Champion("Galio", ["Mid", "Support"], ["tank", "engage", "peel", "cc"]),
            Champion("Akali", ["Mid", "Baron"], ["assassin", "burst", "mobile", "dive"]),
            Champion("Fizz", ["Mid"], ["assassin", "burst", "mobile", "dive"]),
            Champion("Orianna", ["Mid"], ["mage", "scaling", "peel", "immobile"]),
            Champion("Vex", ["Mid"], ["mage", "burst", "anti-dash", "cc"]),
            Champion("Lissandra", ["Mid"], ["mage", "engage", "anti-dash", "cc"]),

            Champion("Jinx", ["Dragon"], ["marksman", "scaling", "immobile", "dragon"]),
            Champion("Kai'Sa", ["Dragon"], ["marksman", "scaling", "mobile", "dive"]),
            Champion("Jhin", ["Dragon"], ["marksman", "poke", "immobile", "dragon"]),
            Champion("Senna", ["Dragon", "Support"], ["marksman", "support", "poke", "scaling"]),
            Champion("Caitlyn", ["Dragon"], ["marksman", "poke", "lane-bully", "dragon"]),
            Champion("Varus", ["Dragon"], ["marksman", "poke", "cc", "immobile"]),
            Champion("Lucian", ["Dragon", "Mid"], ["marksman", "lane-bully", "mobile", "burst"]),
            Champion("Draven", ["Dragon"], ["marksman", "lane-bully", "ad", "dragon"]),
            Champion("Tristana", ["Dragon", "Mid"], ["marksman", "scaling", "mobile", "burst"]),
            Champion("Xayah", ["Dragon"], ["marksman", "scaling", "safe", "cc"]),
            Champion("Miss Fortune", ["Dragon"], ["marksman", "poke", "immobile", "burst"]),

            Champion("Morgana", ["Support", "Mid"], ["mage", "support", "peel", "cc"]),
            Champion("Leona", ["Support"], ["tank", "engage", "cc", "support"]),
            Champion("Nautilus", ["Support"], ["tank", "engage", "cc", "support"]),
            Champion("Thresh", ["Support"], ["support", "engage", "peel", "cc"]),
            Champion("Yuumi", ["Support"], ["support", "peel", "scaling", "safe"]),
            Champion("Braum", ["Support"], ["tank", "peel", "cc", "support"]),
            Champion("Janna", ["Support"], ["support", "peel", "safe", "cc"]),
            Champion("Nami", ["Support"], ["support", "peel", "engage", "cc"]),
            Champion("Karma", ["Support", "Mid"], ["mage", "support", "poke", "peel"]),
            Champion("Alistar", ["Support"], ["tank", "engage", "peel", "cc"]),
            Champion("Rakan", ["Support"], ["support", "engage", "peel", "mobile"]),
            Champion("Pyke", ["Support"], ["assassin", "support", "engage", "mobile"])
        };
    }

    private static IReadOnlyCollection<MatchupRule> InitialMatchupRules()
    {
        return new List<MatchupRule>
        {
            Rule("Baron", "Malphite", "Darius", 30,
                "Armor scaling helps against physical damage.",
                "Play short trades early and avoid extended fights before armor items."),
            Rule("Baron", "Garen", "Darius", 20,
                "Safe lane option with sustain.",
                "Keep trades short and recover between exchanges."),
            Rule("Baron", "Fiora", "Dr. Mundo", 35,
                "True damage punishes tanky sustain champions.",
                "Pressure vital procs and deny comfortable scaling."),
            Rule("Baron", "Vayne", "Dr. Mundo", 35,
                "Tank shred and range punish Mundo.",
                "Maintain spacing and punish every attempt to farm."),
            Rule("Baron", "Renekton", "Yasuo", 24,
                "Reliable early pressure punishes Yasuo before he scales.",
                "Control the wave and force short empowered trades."),
            Rule("Baron", "Malphite", "Yasuo", 28,
                "Armor and reliable engage limit Yasuo's mobility.",
                "Absorb early pressure and set up decisive engages."),
            Rule("Jungle", "Rammus", "Master Yi", 32,
                "Anti-AD durability and crowd control punish Master Yi.",
                "Track his farm route and force fights before he scales."),
            Rule("Jungle", "Vi", "Master Yi", 22,
                "Reliable lockdown prevents Master Yi from freely resetting.",
                "Invade with priority and save lockdown for his engage."),
            Rule("Dragon", "Caitlyn", "Kai'Sa", 22,
                "Range and lane pressure restrict Kai'Sa's early farm.",
                "Use range advantage to control the wave and plates."),
            Rule("Dragon", "Draven", "Senna", 25,
                "Early damage punishes Senna's slower scaling lane.",
                "Force early trades and convert pressure into objectives."),
            Rule("Support", "Leona", "Senna", 25,
                "Hard engage punishes Senna's immobility.",
                "Control brush vision and engage when Senna steps forward."),
            Rule("Support", "Nautilus", "Yuumi", 24,
                "Reliable engage pressures Yuumi's lane partner.",
                "Target the exposed carry and force early summoner spells.")
        };
    }

    private static Champion Champion(string name, List<string> roles, List<string> tags)
    {
        return new Champion { Name = name, Roles = roles, Tags = tags };
    }

    private static MatchupRule Rule(
        string role,
        string champion,
        string enemyChampion,
        int scoreModifier,
        string reason,
        string plan)
    {
        return new MatchupRule
        {
            Role = role,
            Champion = champion,
            EnemyChampion = enemyChampion,
            ScoreModifier = scoreModifier,
            Reason = reason,
            Plan = plan
        };
    }
}
