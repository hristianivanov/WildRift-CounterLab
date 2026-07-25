using Microsoft.EntityFrameworkCore;

using WildRiftCounterLab.Application.Interfaces;
using WildRiftCounterLab.Domain.Entities;
using WildRiftCounterLab.Infrastructure.Data;

namespace WildRiftCounterLab.Infrastructure.Repositories;

public class AiExplanationCacheRepository : IAiExplanationCacheRepository
{
    private readonly ApplicationDbContext _context;

    public AiExplanationCacheRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<AiExplanationCache?> GetByCacheKeyAsync(
        string cacheKey,
        CancellationToken cancellationToken = default)
    {
        return _context.AiExplanationCaches
            .AsNoTracking()
            .SingleOrDefaultAsync(cache => cache.CacheKey == cacheKey, cancellationToken);
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
