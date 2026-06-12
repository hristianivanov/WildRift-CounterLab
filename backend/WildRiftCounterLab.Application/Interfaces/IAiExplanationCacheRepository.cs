using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Interfaces;

public interface IAiExplanationCacheRepository
{
    Task<AiExplanationCache?> GetByCacheKeyAsync(
        string cacheKey,
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        AiExplanationCache cache,
        CancellationToken cancellationToken = default);
}
