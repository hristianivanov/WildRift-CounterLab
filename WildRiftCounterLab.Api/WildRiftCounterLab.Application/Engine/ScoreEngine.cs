using WildRiftCounterLab.Domain.Entities;


namespace WildRiftCounterLab.Application.Engine;

public class ScoreEngine
{
    public int CalculateScore(string champion, List<MatchupRule> rules)
    {
        var score = 50;

        var championRules = rules
            .Where(rule => rule.Champion == champion)
            .ToList();

        foreach (var rule in championRules)
        {
            score += rule.ScoreModifier;
        }

        return score;
    }
}