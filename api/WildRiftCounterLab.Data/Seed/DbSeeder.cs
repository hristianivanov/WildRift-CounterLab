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
            Champion("Pyke", ["Support"], ["assassin", "support", "engage", "mobile"]),

            // Baron / Solo-lane additions
            Champion("Aatrox", ["Baron"], ["fighter", "sustain", "lane-bully", "dive", "ad"]),
            Champion("Cho'Gath", ["Baron", "Mid"], ["tank", "sustain", "scaling", "cc"]),
            Champion("Gnar", ["Baron"], ["fighter", "poke", "cc", "mobile"]),
            Champion("Hecarim", ["Jungle"], ["fighter", "engage", "mobile", "dive"]),
            Champion("Heimerdinger", ["Mid", "Baron"], ["mage", "poke", "immobile"]),
            Champion("K'Sante", ["Baron"], ["tank", "engage", "peel", "cc"]),
            Champion("Kassadin", ["Mid"], ["assassin", "mobile", "scaling", "anti-mage"]),
            Champion("Kayn", ["Jungle"], ["assassin", "mobile", "dive", "sustain"]),
            Champion("Mordekaiser", ["Baron"], ["fighter", "sustain", "scaling", "cc"]),
            Champion("Nocturne", ["Jungle"], ["assassin", "mobile", "dive", "burst"]),
            Champion("Poppy", ["Jungle", "Support", "Baron"], ["tank", "cc", "peel", "anti-dash"]),
            Champion("Rell", ["Support"], ["tank", "engage", "cc", "support"]),
            Champion("Rengar", ["Jungle"], ["assassin", "burst", "mobile", "dive"]),
            Champion("Rumble", ["Baron", "Mid"], ["mage", "poke", "cc"]),
            Champion("Ryze", ["Mid"], ["mage", "scaling", "immobile", "cc"]),
            Champion("Sion", ["Baron", "Support"], ["tank", "cc", "engage", "scaling"]),
            Champion("Skarner", ["Jungle"], ["tank", "engage", "cc", "dive"]),
            Champion("Swain", ["Support", "Mid"], ["mage", "sustain", "cc", "scaling"]),
            Champion("Syndra", ["Mid"], ["mage", "burst", "poke", "cc"]),
            Champion("Talon", ["Mid", "Jungle"], ["assassin", "burst", "mobile", "dive"]),
            Champion("Tryndamere", ["Baron"], ["fighter", "scaling", "mobile", "dive"]),
            Champion("Urgot", ["Baron"], ["fighter", "lane-bully", "tank-shred"]),
            Champion("Viego", ["Jungle"], ["assassin", "mobile", "dive", "scaling"]),
            Champion("Viktor", ["Mid"], ["mage", "poke", "scaling", "immobile"]),
            Champion("Vladimir", ["Mid", "Baron"], ["mage", "sustain", "scaling", "safe"]),
            Champion("Volibear", ["Jungle", "Baron"], ["tank", "engage", "cc", "dive"]),

            // Dragon / ADC additions
            Champion("Aurora", ["Mid", "Baron"], ["mage", "mobile", "burst"]),
            Champion("Kalista", ["Dragon"], ["marksman", "mobile", "poke"]),
            Champion("Kindred", ["Jungle"], ["marksman", "mobile", "scaling"]),
            Champion("Kog'Maw", ["Dragon"], ["marksman", "poke", "scaling", "immobile"]),
            Champion("Nilah", ["Dragon"], ["fighter", "mobile", "scaling", "dive"]),
            Champion("Sivir", ["Dragon"], ["marksman", "poke", "safe", "teamfight"]),
            Champion("Smolder", ["Dragon"], ["marksman", "poke", "scaling"]),
            Champion("Twitch", ["Dragon", "Jungle"], ["marksman", "scaling", "mobile"]),
            Champion("Zeri", ["Dragon"], ["marksman", "mobile", "scaling"]),

            // Support additions
            Champion("Bard", ["Support"], ["support", "peel", "mobile", "cc"]),
            Champion("Brand", ["Support", "Mid"], ["mage", "burst", "poke"]),
            Champion("Fiddlesticks", ["Jungle"], ["mage", "burst", "cc"]),
            Champion("Lillia", ["Jungle"], ["mage", "mobile", "cc", "scaling"]),
            Champion("Maokai", ["Support", "Baron"], ["tank", "cc", "peel", "engage"]),
            Champion("Mel", ["Mid"], ["mage", "poke", "safe", "cc"]),
            Champion("Milio", ["Support"], ["support", "peel", "safe"]),
            Champion("Nidalee", ["Jungle", "Mid"], ["mage", "poke", "mobile"]),
            Champion("Norra", ["Support"], ["support", "peel", "mage"]),
            Champion("Vel'Koz", ["Support", "Mid"], ["mage", "poke", "burst", "immobile"]),
            Champion("Yunara", ["Dragon", "Mid"], ["marksman", "mobile", "burst"]),
            Champion("Zilean", ["Support", "Mid"], ["support", "peel", "cc"])
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

            // Galio
            Tip("Galio", "Veigar", "E",
                "Justice Punch (E) dashes through Veigar's Event Horizon walls — use it to escape the cage before it stuns you."),
            Tip("Galio", "Katarina", "R",
                "Hero's Entrance (R) knock-up interrupts Katarina's Death Lotus (R) channel immediately."),
            Tip("Galio", "Zed", "W",
                "Colossal Smash (W) taunt forces Zed's shadows to auto-attack Galio, wasting his Death Mark damage window."),
            Tip("Galio", "Yasuo", "R",
                "Hero's Entrance (R) launches Yasuo airborne — combine with an ally knock-up setup for maximum chain CC."),
            Tip("Galio", "Annie", "E",
                "Justice Punch (E) dash can dodge Annie's Tibbers cast or reposition away from the stun radius."),

            // Lux
            Tip("Lux", "Yasuo", "Q",
                "Light Binding (Q) roots Yasuo in place — throw it when he dashes through a minion so he can't immediately dash away again."),
            Tip("Lux", "Zed", "W",
                "Prismatic Barrier (W) shield can absorb a large portion of Zed's Death Mark detonation — activate it just after the mark lands."),
            Tip("Lux", "Katarina", "Q",
                "Light Binding (Q) roots Katarina out of Death Lotus (R) if she's channeling — or use it the moment she jumps in."),
            Tip("Lux", "Morgana", "E",
                "Lucent Singularity (E) placed near Morgana's feet forces her to move, making her Dark Binding (Q) harder to land."),

            // Ahri
            Tip("Ahri", "Veigar", "R",
                "Spirit Rush (R) dashes can escape Veigar's Event Horizon cage — save at least one charge for his combo."),
            Tip("Ahri", "Zed", "R",
                "Use the third Spirit Rush (R) dash reactively when Zed's Death Mark detonates to dodge the burst zone."),
            Tip("Ahri", "Fizz", "E",
                "Charm (E) locks Fizz down before he can use Playful/Trickster — land it the moment he steps forward to trade."),
            Tip("Ahri", "Katarina", "E",
                "Charm (E) cancels Katarina's Death Lotus (R) channel — bait her jump in then immediately cast E."),

            // Orianna
            Tip("Orianna", "Yasuo", "R",
                "Command: Shockwave (R) launches Yasuo airborne — coordinate with a team knock-up to chain his own passive for ally Yasuo R."),
            Tip("Orianna", "Zed", "W",
                "Command: Protect (W) shield on yourself absorbs a significant portion of Zed's Death Mark burst if timed on impact."),
            Tip("Orianna", "Katarina", "R",
                "Shockwave (R) interrupts Katarina's Death Lotus (R) — keep the ball near her jump target to cast instantly."),

            // Lissandra
            Tip("Lissandra", "Yasuo", "E",
                "Glacial Path (E) passes through Yasuo's Wind Wall — use it to engage safely without your spells being blocked."),
            Tip("Lissandra", "Zed", "R",
                "Frozen Tomb (R) on yourself makes Lissandra untargetable during Zed's Death Mark detonation — activate immediately after the mark lands."),
            Tip("Lissandra", "Katarina", "Q",
                "Ice Shard (Q) shatters into multiple pieces — hitting Katarina through minions can stack her slow and set up a follow-up Claw (E)."),

            // Vex
            Tip("Vex", "Yasuo", "E",
                "Shadow Surge (E) hits Yasuo after he dashes, automatically granting the Fear Beyond Death (R) reset if he's low — track his dash count."),
            Tip("Vex", "Akali", "Q",
                "Mistral Bolt (Q) applies Gloom on Akali when she dashes, triggering an empowered auto — punish every shroud exit."),
            Tip("Vex", "Zed", "passive",
                "Every time Zed shadow-dashes, Vex's Doom 'n Gloom (passive) marks him — proc the mark immediately for bonus damage and an auto-reset."),
            Tip("Vex", "Fizz", "Q",
                "Mistral Bolt (Q) accelerates over distance — fire it right as Fizz uses Playful/Trickster (E) so it hits him on landing."),

            // Vladimir
            Tip("Vladimir", "Zed", "W",
                "Sanguine Pool (W) makes Vladimir untargetable — activate it immediately after Zed's Death Mark lands to completely dodge the detonation."),
            Tip("Vladimir", "Veigar", "W",
                "Sanguine Pool (W) dodges Event Horizon's stun and Primordial Burst (R) — pool the moment you see the cage forming."),
            Tip("Vladimir", "Fizz", "W",
                "Sanguine Pool (W) avoids Fizz's Chum the Waters (R) shark hit and removes the mark before it detonates."),
            Tip("Vladimir", "Annie", "W",
                "Pool (W) during Annie's Tibbers stun animation to become untargetable and reposition before the damage arrives."),

            // Morgana
            Tip("Morgana", "Veigar", "E",
                "Black Shield (E) on yourself or an ally fully blocks Veigar's Event Horizon stun — cast it the moment the cage begins forming."),
            Tip("Morgana", "Leona", "E",
                "Black Shield (E) blocks Leona's Zenith Blade (E) root and Solar Flare (R) stun — prioritise shielding the carry."),
            Tip("Morgana", "Nautilus", "E",
                "Black Shield absorbs Nautilus's Dredge Line (Q) hook and Depth Charge (R) knock-up entirely."),
            Tip("Morgana", "Lissandra", "E",
                "Black Shield (E) blocks all of Lissandra's CC chain — cast it before she initiates with Glacial Path."),

            // Malphite (extra tips)
            Tip("Malphite", "Zed", "passive",
                "Malphite's passive Granite Shield regenerates between Zed's poke windows — stack armor items to make his AD damage negligible."),
            Tip("Malphite", "Caitlyn", "passive",
                "Granite Shield absorbs Caitlyn's Headshot procs — let the shield regenerate between her empowered shots to minimize poke damage."),
            Tip("Malphite", "Darius", "R",
                "Unstoppable Force (R) interrupts Darius's Noxian Guillotine (R) channel — use it the moment he jumps on your carry."),

            // Leona
            Tip("Leona", "Caitlyn", "Q",
                "Shield of Daybreak (Q) stuns Caitlyn right after gap-closing — cancel her Headshot setup and force a Flash or summoner."),
            Tip("Leona", "Jhin", "W",
                "Eclipse (W) movement speed burst lets Leona close the gap on Jhin even without minion walls — dash through terrain brush."),
            Tip("Leona", "Jinx", "E",
                "Zenith Blade (E) dashes through Jinx's Flame Chompers (E) traps without triggering them."),
            Tip("Leona", "Miss Fortune", "R",
                "Solar Flare (R) or Zenith Blade (E) interrupts Miss Fortune's Bullet Time (R) channel — engage as soon as she begins casting."),

            // Nautilus
            Tip("Nautilus", "Caitlyn", "Q",
                "Dredge Line (Q) hook pulls Nautilus to Caitlyn — cancel her Headshot winddown immediately with Riptide (E) after landing."),
            Tip("Nautilus", "Jinx", "R",
                "Depth Charge (R) knock-up interrupts Jinx's Super Mega Death Rocket (R) channel if she's in the AoE."),
            Tip("Nautilus", "Jhin", "Q",
                "Dredge Line (Q) to terrain closes the gap on Jhin and brings him into stasis range before Curtain Call (R) finishes."),

            // Thresh
            Tip("Thresh", "Caitlyn", "E",
                "Flay (E) interrupts Caitlyn's 90 Caliber Net (E) repositioning — use it as she activates the net to cancel her escape."),
            Tip("Thresh", "Jinx", "Q",
                "Death Sentence (Q) hook on Jinx during her Super Mega Death Rocket (R) channel immediately cancels the cast."),
            Tip("Thresh", "Miss Fortune", "E",
                "Flay (E) or Death Sentence (Q) interrupts Bullet Time (R) — Thresh has two tools to cancel her channel."),

            // Katarina
            Tip("Katarina", "Lux", "E",
                "Shunpo (E) jumps to Lux's ward or a nearby dagger to dodge her Final Spark (R) beam if you can reposition diagonally."),
            Tip("Katarina", "Veigar", "E",
                "Shunpo (E) to a dagger outside Veigar's Event Horizon can let you escape the cage before it fully stuns."),

            // Akali
            Tip("Akali", "Veigar", "W",
                "Twilight Shroud (W) makes Akali invisible inside the smoke — drop it inside Veigar's cage to wait out the stun duration."),
            Tip("Akali", "Zed", "W",
                "Twilight Shroud (W) causes Zed's Living Shadow to lose track of Akali — use it immediately after Death Mark lands."),
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