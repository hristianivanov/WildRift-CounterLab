using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Interfaces;

public interface IMatchupRuleRepository
{
    Task<List<MatchupRule>> GetRulesForDraftAsync(string role, List<string> enemies);

    Task<List<MatchupRule>> GetAllAsync();

    Task<MatchupRule?> GetByIdAsync(int id);

    Task AddAsync(MatchupRule rule);

    Task UpdateAsync(MatchupRule rule);

    Task DeleteAsync(MatchupRule rule);

    Task<bool> ExistsAsync(string role, string champion, string enemyChampion);

    Task<bool> ExistsForChampionAsync(string championName);
}