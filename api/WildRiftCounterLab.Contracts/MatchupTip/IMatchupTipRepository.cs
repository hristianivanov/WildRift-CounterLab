namespace WildRiftCounterLab.Contracts;

using WildRiftCounterLab.Data.Models;

public interface IMatchupTipRepository
{
    Task<List<MatchupTip>> GetTipsForDraftAsync(string champion, List<string> enemies, CancellationToken cancellationToken = default);

    Task<List<MatchupTip>> GetTipsForChampionsAsync(List<string> champions, List<string> enemies, CancellationToken cancellationToken = default);

    Task<List<MatchupTip>> GetAllAsync();

    Task<MatchupTip?> GetByIdAsync(int id);

    Task AddAsync(MatchupTip tip);

    Task UpdateAsync(MatchupTip tip);

    Task DeleteAsync(MatchupTip tip);

    Task<bool> ExistsAsync(string champion, string enemyChampion, string tip);
}
