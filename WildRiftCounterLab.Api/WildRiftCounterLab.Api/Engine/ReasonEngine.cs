namespace WildRiftCounterLab.Api.Engine;

using Models;

public class ReasonEngine
{
    public List<string> BuildReasons(string champion, List<MatchupRule> rules)
    {
        var reasons = rules
            .Where(rule => rule.Champion == champion)
            .Select(rule => rule.Reason)
            .Distinct()
            .ToList();

        if (reasons.Any())
        {
            return reasons;
        }

        return new List<string>
        {
            "Solid general pick"
        };
    }
}