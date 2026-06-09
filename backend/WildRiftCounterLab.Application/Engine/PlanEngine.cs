using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Engine;

public class PlanEngine
{
    public string BuildPlan(Champion champion, IReadOnlyCollection<MatchupRule> rules)
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