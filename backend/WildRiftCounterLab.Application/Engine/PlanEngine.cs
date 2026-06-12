using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Engine;

public class PlanEngine
{
    public string BuildPlan(
        Champion champion,
        IReadOnlyCollection<MatchupRule> rules,
        IReadOnlyCollection<Champion>? enemies = null)
    {
        var plans = rules
            .Where(rule => rule.Champion.Equals(champion.Name, StringComparison.OrdinalIgnoreCase))
            .Select(rule => rule.Plan)
            .Where(plan => !string.IsNullOrWhiteSpace(plan))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (plans.Count > 0)
        {
            return string.Join(" ", plans);
        }

        var profile = EnemyDraftProfile.Create(enemies ?? Array.Empty<Champion>());

        if (profile.PokeComp)
        {
            return "Preserve health through the enemy poke, avoid slow sieges, and engage when key ranged abilities are unavailable.";
        }

        if (profile.MobileComp)
        {
            return "Hold key control tools for enemy movement, avoid isolated chases, and force fights in constrained areas.";
        }

        if (profile.DiveComp)
        {
            return "Track enemy engage threats, stay close enough to protect carries, and punish divers after they commit.";
        }

        if (profile.TankHeavy)
        {
            return "Avoid wasting cooldowns on the frontline, preserve damage for extended fights, and focus priority targets when access opens.";
        }

        if (profile.ScalingComp)
        {
            return "Create early objective pressure, deny free scaling, and convert tempo advantages before late-game power spikes.";
        }

        if (champion.Tags.Contains("lane-bully", StringComparer.OrdinalIgnoreCase))
        {
            return "Pressure the lane early, control the wave, and convert the lead into objectives.";
        }

        if (champion.Tags.Contains("scaling", StringComparer.OrdinalIgnoreCase))
        {
            return "Avoid unnecessary early risks, farm consistently, and play around later power spikes.";
        }

        if (champion.Tags.Contains("engage", StringComparer.OrdinalIgnoreCase))
        {
            return "Preserve health, look for grouped enemies, and start fights when allies can follow.";
        }

        if (champion.Tags.Contains("safe", StringComparer.OrdinalIgnoreCase))
        {
            return "Keep trades controlled, protect your resources, and punish enemy overextensions.";
        }

        return "Play a stable lane, track enemy threats, and fight around your strongest teammates.";
    }
}
