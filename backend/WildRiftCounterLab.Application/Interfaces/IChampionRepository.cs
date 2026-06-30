using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Interfaces;

public interface IChampionRepository
{
    Task<List<Champion>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Champion?> GetByIdAsync(int id);

    Task<Champion?> GetByNameAsync(string name);

    Task AddAsync(Champion champion);

    Task UpdateAsync(Champion champion);

    Task DeleteAsync(Champion champion);

    Task<bool> ExistsByNameAsync(string name);
}