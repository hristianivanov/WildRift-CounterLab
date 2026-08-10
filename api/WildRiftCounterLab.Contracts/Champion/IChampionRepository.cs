namespace WildRiftCounterLab.Contracts;

using WildRiftCounterLab.Data.Models;

public interface IChampionRepository
{
    Task<List<Champion>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Champion?> GetByIdAsync(int id);

    Task AddAsync(Champion champion);

    Task AddRangeAsync(IEnumerable<Champion> champions, CancellationToken cancellationToken = default);

    Task UpdateAsync(Champion champion);

    Task DeleteAsync(Champion champion);

    Task DeleteRangeAsync(IEnumerable<Champion> champions, CancellationToken cancellationToken = default);

    Task<bool> ExistsByNameAsync(string name);
}