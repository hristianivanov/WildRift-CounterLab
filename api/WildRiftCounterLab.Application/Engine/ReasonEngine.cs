using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Engine;

public class ReasonEngine
{
    public List<string> BuildReasons(
        Champion champion,
        IReadOnlyCollection<Champion> enemies,
        IReadOnlyCollection<MatchupRule> rules)
    {
        var ruleReasons = rules
            .Where(rule => rule.Champion.Equals(champion.Name, StringComparison.OrdinalIgnoreCase))
            .Select(rule => rule.Reason)
            .Where(reason => !string.IsNullOrWhiteSpace(reason))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(3)
            .ToList();

        var championTags = champion.Tags.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var profile = EnemyDraftProfile.Create(enemies);
        var profileReasons = BuildProfileReasons(championTags, profile);

        var reasons = ruleReasons
            .Concat(profileReasons)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(4)
            .ToList();

        if (reasons.Count == 0)
        {
            reasons.Add("Solid role fit with a dependable general game plan.");
        }

        return reasons;
    }

    private static List<string> BuildProfileReasons(
        HashSet<string> championTags,
        EnemyDraftProfile profile)
    {
        var reasons = new List<string>();

        AddReason(reasons, championTags.Contains("anti-ad") && profile.HeavyAd,
            "Anti-AD tools gain value into the enemy team's heavy physical damage profile.");
        AddReason(reasons, championTags.Contains("anti-ap") && profile.HeavyAp,
            "Anti-AP tools gain value into the enemy team's heavy magic damage profile.");
        AddReason(reasons, championTags.Contains("tank-shred") && profile.TankHeavy,
            "Tank shred directly answers the enemy team's tank-heavy frontline.");
        AddReason(reasons, championTags.Contains("true-damage") && profile.DurableEnemyCount > 0,
            "True damage stays effective into durable tanks and fighters.");
        AddReason(reasons, championTags.Contains("engage") &&
            (profile.SquishyBackline || profile.ImmobileCarries),
            "Reliable engage can reach the enemy team's vulnerable backline.");
        AddReason(reasons, championTags.Contains("peel") && profile.DiveComp,
            "Peel protects allied carries from the enemy team's dive threats.");
        AddReason(reasons, championTags.Contains("anti-dash") && profile.MobileComp,
            "Anti-dash tools punish the enemy team's mobile composition.");
        AddReason(reasons, championTags.Contains("sustain") && profile.PokeComp,
            "Sustain helps absorb the enemy team's repeated poke.");
        AddReason(reasons, championTags.Contains("safe") && profile.BurstOrPickComp,
            "Safe positioning reduces the threat from burst and pick tools.");
        AddReason(reasons, championTags.Contains("teamfight") && profile.GroupedFightComp,
            "Teamfight tools add value around grouped objective fights.");
        AddReason(reasons, championTags.Contains("scaling") && profile.ScalingComp,
            "Strong scaling keeps pace with the enemy team's late-game profile.");

        return reasons;
    }

    private static void AddReason(List<string> reasons, bool condition, string reason)
    {
        if (condition)
        {
            reasons.Add(reason);
        }
    }
}
