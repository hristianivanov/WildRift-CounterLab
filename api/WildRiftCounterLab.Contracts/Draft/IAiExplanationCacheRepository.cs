using WildRiftCounterLab.Data.Models;

namespace WildRiftCounterLab.Contracts;

public interface IAiExplanationCacheRepository
{
    Task<AiExplanationCache?> GetByCacheKeyAsync(
        string cacheKey,
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        AiExplanationCache cache,
        CancellationToken cancellationToken = default);
}
