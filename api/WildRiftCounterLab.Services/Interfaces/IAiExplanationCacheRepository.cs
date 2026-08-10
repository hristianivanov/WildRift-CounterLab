using WildRiftCounterLab.Data.Models;

namespace WildRiftCounterLab.Services.Interfaces;

public interface IAiExplanationCacheRepository
{
    Task<AiExplanationCache?> GetByCacheKeyAsync(
        string cacheKey,
        CancellationToken cancellationToken = default);

    Task SaveAsync(
        AiExplanationCache cache,
        CancellationToken cancellationToken = default);
}
