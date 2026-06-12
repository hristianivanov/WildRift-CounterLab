using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Exceptions;
using WildRiftCounterLab.Application.Interfaces;
using WildRiftCounterLab.Application.Services;
using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Infrastructure.AI;

public class CachedAiExplanationProvider : IAiExplanationProvider
{
    public const string RateLimitFallback = "AI explanation unavailable due to provider rate limit.";

    private readonly IExternalAiExplanationProvider _provider;
    private readonly IAiExplanationCacheRepository _cacheRepository;

    public CachedAiExplanationProvider(
        IExternalAiExplanationProvider provider,
        IAiExplanationCacheRepository cacheRepository)
    {
        _provider = provider;
        _cacheRepository = cacheRepository;
    }

    public async Task<AiExplanationResponseDto> ExplainAsync(AiExplanationRequestDto request)
    {
        var cacheKey = AiExplanationCacheKeyBuilder.Build(request);
        var cached = await TryGetCachedAsync(cacheKey);

        if (cached is not null)
        {
            return new AiExplanationResponseDto { Explanation = cached.Explanation };
        }

        try
        {
            var response = await _provider.ExplainAsync(request);
            await TrySaveAsync(request, cacheKey, response.Explanation);

            return response;
        }
        catch (AiProviderRateLimitException)
        {
            return new AiExplanationResponseDto { Explanation = RateLimitFallback };
        }
        catch (Exception)
        {
            return new AiExplanationResponseDto { Explanation = BuildFallback(request) };
        }
    }

    public async Task<IReadOnlyDictionary<string, string>> ExplainBatchAsync(
        IReadOnlyCollection<AiExplanationRequestDto> requests,
        CancellationToken cancellationToken = default)
    {
        var explanations = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var cacheMisses = new List<(AiExplanationRequestDto Request, string CacheKey)>();

        foreach (var request in requests)
        {
            var cacheKey = AiExplanationCacheKeyBuilder.Build(request);
            var cached = await TryGetCachedAsync(cacheKey, cancellationToken);

            if (cached is not null)
            {
                explanations[request.Champion] = cached.Explanation;
            }
            else
            {
                cacheMisses.Add((request, cacheKey));
            }
        }

        if (cacheMisses.Count == 0)
        {
            return explanations;
        }

        try
        {
            var generated = await _provider.ExplainBatchAsync(
                cacheMisses.Select(item => item.Request).ToList(),
                cancellationToken);

            foreach (var item in cacheMisses)
            {
                if (generated.TryGetValue(item.Request.Champion, out var explanation)
                    && !string.IsNullOrWhiteSpace(explanation))
                {
                    explanations[item.Request.Champion] = explanation;
                    await TrySaveAsync(item.Request, item.CacheKey, explanation, cancellationToken);
                }
                else
                {
                    explanations[item.Request.Champion] = BuildFallback(item.Request);
                }
            }
        }
        catch (AiProviderRateLimitException)
        {
            foreach (var item in cacheMisses)
            {
                explanations[item.Request.Champion] = RateLimitFallback;
            }
        }
        catch (Exception)
        {
            foreach (var item in cacheMisses)
            {
                explanations[item.Request.Champion] = BuildFallback(item.Request);
            }
        }

        return explanations;
    }

    private async Task<AiExplanationCache?> TryGetCachedAsync(
        string cacheKey,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _cacheRepository.GetByCacheKeyAsync(cacheKey, cancellationToken);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task TrySaveAsync(
        AiExplanationRequestDto request,
        string cacheKey,
        string explanation,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _cacheRepository.SaveAsync(
                new AiExplanationCache
                {
                    CacheKey = cacheKey,
                    Role = request.Role,
                    LaneEnemy = request.LaneEnemy,
                    EnemyTeamHash = AiExplanationCacheKeyBuilder.BuildEnemyTeamHash(request.EnemyTeam),
                    Champion = request.Champion,
                    Explanation = explanation,
                    CreatedAt = DateTime.UtcNow
                },
                cancellationToken);
        }
        catch (Exception)
        {
            // Cache persistence is optional and must never break AI explanations.
        }
    }

    private static string BuildFallback(AiExplanationRequestDto request)
    {
        var reasons = request.Reasons.Count > 0
            ? string.Join(" ", request.Reasons.Take(2))
            : "This champion fits the deterministic recommendation engine's draft evaluation.";

        return $"AI explanation is unavailable right now. Engine summary: {reasons} Plan: {request.Plan}";
    }
}
