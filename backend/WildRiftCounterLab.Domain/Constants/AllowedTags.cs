namespace WildRiftCounterLab.Domain.Constants;

public static class AllowedTags
{
    public static readonly HashSet<string> Values = new(StringComparer.OrdinalIgnoreCase)
    {
        // Damage profile
        "anti-ad",
        "anti-ap",
        "tank-shred",
        "true-damage",

        // Lane dynamics
        "lane-bully",
        "scaling",

        // Safety
        "safe",
        "sustain",

        // Utility / team
        "engage",
        "peel",
        "anti-dash",
        "teamfight",

        // Enemy composition labels (used by EnemyDraftProfile to classify enemy team)
        "poke",
        "burst",
        "pick",
        "immobile",
        "dive",

        // General labels (used in reason/plan engines but not scored directly)
        "tank",
        "fighter",
        "marksman",
        "assassin",
        "mage",
        "support",
        "mobile",
    };
}
