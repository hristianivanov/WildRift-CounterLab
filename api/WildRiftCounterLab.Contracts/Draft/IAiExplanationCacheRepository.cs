namespace WildRiftCounterLab.Contracts;

using WildRiftCounterLab.Data.Models;

public interface IAiExplanationCacheRepository
{
    Task<AiExplanationCache?> GetByCacheKeyAsync(
        string cacheKey,
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        AiExplanationCache cache,
        CancellationToken cancellationToken = default);
}