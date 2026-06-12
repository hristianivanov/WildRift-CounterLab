using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Engine;

public sealed class EnemyDraftProfile
{
    private EnemyDraftProfile()
    {
    }

    public bool HeavyAd { get; private init; }

    public bool HeavyAp { get; private init; }

    public bool MixedDamage { get; private init; }

    public bool TankHeavy { get; private init; }

    public bool SquishyBackline { get; private init; }

    public bool MobileComp { get; private init; }

    public bool DiveComp { get; private init; }

    public bool PokeComp { get; private init; }

    public bool ScalingComp { get; private init; }

    public bool LowFrontlineComp { get; private init; }

    public bool ImmobileCarries { get; private init; }

    public bool BurstOrPickComp { get; private init; }

    public bool GroupedFightComp { get; private init; }

    public int DurableEnemyCount { get; private init; }

    public static EnemyDraftProfile Create(IReadOnlyCollection<Champion> enemies)
    {
        var enemyList = enemies.ToList();
        var heavyDamageThreshold = Math.Max(2, (int)Math.Ceiling(enemyList.Count * 0.6));
        var compositionThreshold = Math.Max(2, (int)Math.Ceiling(enemyList.Count * 0.4));
        var adCount = CountChampions(enemyList, "ad", "marksman");
        var apCount = CountChampions(enemyList, "ap", "mage");
        var tankCount = CountChampions(enemyList, "tank");
        var fighterCount = CountChampions(enemyList, "fighter", "juggernaut");
        var carryCount = CountChampions(enemyList, "marksman", "mage");
        var frontlineCount = CountChampions(enemyList, "tank", "fighter", "juggernaut");

        var tankHeavy = tankCount >= compositionThreshold;
        var scalingComp = CountChampions(enemyList, "scaling") >= compositionThreshold;
        var diveComp = CountChampions(enemyList, "dive", "assassin") >= compositionThreshold;

        return new EnemyDraftProfile
        {
            HeavyAd = adCount >= heavyDamageThreshold,
            HeavyAp = apCount >= heavyDamageThreshold,
            MixedDamage = adCount > 0 && apCount > 0 && !(
                adCount >= heavyDamageThreshold || apCount >= heavyDamageThreshold),
            TankHeavy = tankHeavy,
            SquishyBackline = carryCount >= compositionThreshold && tankCount <= 1,
            MobileComp = CountChampions(enemyList, "mobile") >= compositionThreshold,
            DiveComp = diveComp,
            PokeComp = CountChampions(enemyList, "poke") >= compositionThreshold,
            ScalingComp = scalingComp,
            LowFrontlineComp = enemyList.Count >= 2 && frontlineCount <= 1,
            ImmobileCarries = enemyList.Any(enemy =>
                HasAnyTag(enemy, "marksman", "mage") && HasAnyTag(enemy, "immobile")),
            BurstOrPickComp = CountChampions(enemyList, "burst", "pick") >= compositionThreshold,
            GroupedFightComp =
                tankHeavy ||
                scalingComp ||
                diveComp ||
                CountChampions(enemyList, "teamfight") >= compositionThreshold,
            DurableEnemyCount = tankCount + fighterCount
        };
    }

    private static int CountChampions(IEnumerable<Champion> champions, params string[] tags)
    {
        return champions.Count(champion => HasAnyTag(champion, tags));
    }

    private static bool HasAnyTag(Champion champion, params string[] tags)
    {
        return tags.Any(tag => champion.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase));
    }
}
