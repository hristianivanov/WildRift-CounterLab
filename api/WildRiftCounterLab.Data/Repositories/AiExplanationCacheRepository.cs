namespace WildRiftCounterLab.Data.Repositories;

using Microsoft.EntityFrameworkCore;

using WildRiftCounterLab.Contracts;
using WildRiftCounterLab.Data.Models;

public class AiExplanationCacheRepository : IAiExplanationCacheRepository
{
    private readonly ApplicationDbContext _context;

    public AiExplanationCacheRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    private static readonly TimeSpan CacheTtl = TimeSpan.FromDays(30);

    public Task<AiExplanationCache?> GetByCacheKeyAsync(
        string cacheKey,
        CancellationToken cancellationToken = default)
    {
        var expiryThreshold = DateTime.UtcNow - CacheTtl;
        return _context.AiExplanationCaches
            .AsNoTracking()
            .SingleOrDefaultAsync(
                cache => cache.CacheKey == cacheKey && cache.CreatedAt >= expiryThreshold,
                cancellationToken);
    }

    public async Task InvalidateAllAsync(CancellationToken cancellationToken = default)
    {
        await _context.AiExplanationCaches.ExecuteDeleteAsync(cancellationToken);
    }

    public async Task SaveAsync(
        AiExplanationCache cache,
        CancellationToken cancellationToken = default)
    {
        // The unique DB constraint on CacheKey prevents duplicate inserts;
        // callers wrap this in try/catch so a race-condition conflict is harmless.
        _context.AiExplanationCaches.Add(cache);
        await _context.SaveChangesAsync(cancellationToken);
    }
}