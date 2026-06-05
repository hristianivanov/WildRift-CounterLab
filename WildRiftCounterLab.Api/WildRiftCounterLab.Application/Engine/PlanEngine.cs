using WildRiftCounterLab.Domain.Entities;


namespace WildRiftCounterLab.Application.Engine;

public class PlanEngine
{
    public string BuildPlan(string champion, List<MatchupRule> rules)
    {
        var plans = rules
            .Where(rule => rule.Champion == champion)
            .Select(rule => rule.Plan)
            .Where(plan => !string.IsNullOrWhiteSpace(plan))
            .Distinct()
            .ToList();

        if (plans.Any())
        {
            return string.Join(" ", plans);
        }

        return "Play stable early game and scale into mid game fights.";
    }
}