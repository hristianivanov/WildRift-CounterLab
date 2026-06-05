using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Interfaces;

public interface IMatchupRuleRepository
{
    Task<List<MatchupRule>> GetRulesForDraftAsync(string role, List<string> enemies);
}