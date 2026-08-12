namespace WildRiftCounterLab.Data.Seed;

using WildRiftCounterLab.Data.Models;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext db)
    {
        AddMissingChampions(db);
        AddMissingMatchupRules(db);
        AddMissingMatchupTips(db);
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
            Champion("Veigar", ["Mid"], ["mage", "burst", "immobile", "cc"]),
            Champion("Taliyah", ["Mid", "Jungle"], ["mage", "burst", "mobile", "cc"]),
            Champion("Cho'Gath", ["Baron", "Mid"], ["tank", "sustain", "scaling", "cc"]),

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

    private static void AddMissingMatchupTips(ApplicationDbContext db)
    {
        var existingKeys = db.MatchupTips
            .AsEnumerable()
            .Select(TipKey)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missingTips = InitialMatchupTips()
            .Where(tip => existingKeys.Add(TipKey(tip)))
            .ToList();

        if (missingTips.Count > 0)
        {
            db.MatchupTips.AddRange(missingTips);
        }
    }

    private static string TipKey(MatchupTip tip)
    {
        return $"{tip.Champion}|{tip.EnemyChampion}|{tip.Tip}";
    }

    private static IReadOnlyCollection<MatchupTip> InitialMatchupTips()
    {
        return new List<MatchupTip>
        {
            // Fizz
            Tip("Fizz", "Veigar", "E",
                "Use Playful/Trickster (E) to hop over Veigar's Event Horizon cage — time it just before the walls fully form."),
            Tip("Fizz", "Annie", "E",
                "Playful/Trickster (E) makes Fizz untargetable, dodging Annie's Tibbers stun entirely."),
            Tip("Fizz", "Lux", "E",
                "Hop over or dodge Lux's Lucent Singularity (E) and Final Spark (R) with Playful/Trickster."),
            Tip("Fizz", "Morgana", "E",
                "Fizz E removes the binding from Dark Binding (Q) mid-flight if cast early enough."),

            // Nocturne
            Tip("Nocturne", "Veigar", "W",
                "Nocturne's Shroud of Darkness (W) spell shield blocks Veigar's Event Horizon stun — activate it right as the cage appears."),
            Tip("Nocturne", "Lux", "W",
                "Spell shield absorbs Lux's Light Binding (Q) or Final Spark (R) — save W specifically for her R."),
            Tip("Nocturne", "Morgana", "W",
                "Shroud of Darkness (W) absorbs Dark Binding (Q) completely, nullifying her main CC tool."),

            // Yasuo
            Tip("Yasuo", "Veigar", "W",
                "Wind Wall (W) can block Veigar's Baleful Strike (Q) and Primordial Burst (R) projectiles — position it between you and Veigar."),
            Tip("Yasuo", "Jinx", "W",
                "Wind Wall blocks Jinx's Fishbones rockets and Super Mega Death Rocket (R) — use it as soon as she fires."),
            Tip("Yasuo", "Caitlyn", "W",
                "Wind Wall blocks Caitlyn's Piltover Peacemaker (Q) and Ace in the Hole (R)."),
            Tip("Yasuo", "Jhin", "W",
                "Wind Wall blocks all of Jhin's Curtain Call (R) shots — deploy it when he begins channeling."),
            Tip("Yasuo", "Miss Fortune", "W",
                "Wind Wall completely negates Miss Fortune's Bullet Time (R) — place it directly in the cone."),
            Tip("Yasuo", "Draven", "W",
                "Wind Wall blocks Draven's Whirling Death (R) axes on the way in AND the return."),
            Tip("Yasuo", "Lux", "W",
                "Wind Wall absorbs Final Spark (R) — react to the cast animation, not the beam."),

            // Braum
            Tip("Braum", "Jinx", "E",
                "Unbreakable (E) fully absorbs Jinx's Super Mega Death Rocket (R) if you stand between it and your carry."),
            Tip("Braum", "Caitlyn", "E",
                "Unbreakable (E) blocks Ace in the Hole (R) — face toward Caitlyn to intercept it for your carry."),
            Tip("Braum", "Jhin", "E",
                "Unbreakable absorbs all Curtain Call (R) shots — activate and face Jhin when he starts channeling."),
            Tip("Braum", "Miss Fortune", "E",
                "Unbreakable negates Bullet Time (R) — stand in the cone and face her to tank the entire channel."),

            // Garen
            Tip("Garen", "Lissandra", "passive",
                "Garen's passive regeneration is not interrupted by Lissandra's frost slow — keep fighting through the slow, your regen still ticks."),
            Tip("Garen", "Teemo", "Q",
                "Decisive Strike (Q) cleanses Teemo's blind on cast — activate Q immediately after being blinded."),

            // Malphite
            Tip("Malphite", "Yasuo", "R",
                "Unstoppable Force (R) launches Yasuo airborne, setting up your team's abilities and enabling Yasuo's own R if he's allied — but against enemy Yasuo, it hard counters his dashes."),

            // Alistar
            Tip("Alistar", "Veigar", "QW",
                "Headbutt–Pulverize (W–Q) combo can knock Veigar out of his own Event Horizon cage before he casts it."),

            // Xayah
            Tip("Xayah", "Veigar", "R",
                "Clean Cuts (R) makes Xayah untargetable, completely avoiding Veigar's Event Horizon and Primordial Burst (R)."),
            Tip("Xayah", "Caitlyn", "R",
                "Recall (R) dodges Caitlyn's Ace in the Hole (R) if activated before the shot arrives."),

            // Zed
            Tip("Zed", "Lux", "R",
                "Living Shadow (R) swaps can reposition Zed away from Lux's Final Spark — bait the cast then swap."),
            Tip("Zed", "Annie", "R",
                "Death Mark (R) makes Zed untargetable during the leap — use it to dodge Annie's Tibbers stun cast."),

            // Ekko
            Tip("Ekko", "Veigar", "R",
                "Chronobreak (R) teleports Ekko to his ghost position from 3 seconds ago — if you stepped into Veigar's cage, R out immediately."),

            // Shen
            Tip("Shen", "Zed", "E",
                "Shadow Dash (E) taunts Zed, forcing his auto-attacks onto Shen and wasting his damage window during Death Mark (R)."),

            // Wukong
            Tip("Wukong", "Teemo", "W",
                "Warrior Trickster (W) clone can tank Teemo's poison shrooms — use the decoy to find and trigger shrooms safely."),

            // Janna
            Tip("Janna", "Katarina", "R",
                "Monsoon (R) interrupts and pushes Katarina out of her Death Lotus (R) channel — react to the spinning animation."),

            // Gragas
            Tip("Gragas", "Katarina", "E",
                "Body Slam (E) interrupts Katarina's Death Lotus (R) channel immediately."),
        };
    }

    private static MatchupTip Tip(string champion, string enemyChampion, string abilityTag, string tip)
    {
        return new MatchupTip
        {
            Champion = champion,
            EnemyChampion = enemyChampion,
            AbilityTag = abilityTag,
            Tip = tip
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