using Microsoft.Extensions.Logging.Abstractions;

using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Exceptions;
using WildRiftCounterLab.Application.Interfaces;
using WildRiftCounterLab.Application.Services;
using WildRiftCounterLab.Domain.Entities;
using WildRiftCounterLab.Infrastructure.AI;

namespace WildRiftCounterLab.Api.Tests;

public class CachedAiExplanationProviderTests
{
    [Fact]
    public async Task ExplainAsync_CachedExplanationAvoidsProviderCall()
    {
        var request = CreateRequest();
        var cache = new FakeCacheRepository();
        cache.Items.Add(new AiExplanationCache
        {
            CacheKey = AiExplanationCacheKeyBuilder.Build(request),
            Explanation = "Cached explanation"
        });
        var provider = new FakeGeminiProvider();
        var service = new CachedAiExplanationProvider(provider, cache, NullLogger<CachedAiExplanationProvider>.Instance);

        var response = await service.ExplainAsync(request);

        Assert.Equal("Cached explanation", response.Explanation);
        Assert.Equal(0, provider.CallCount);
    }

    [Fact]
    public async Task ExplainAsync_CacheMissCallsProviderAndSavesResult()
    {
        var cache = new FakeCacheRepository();
        var provider = new FakeGeminiProvider();
        var service = new CachedAiExplanationProvider(provider, cache, NullLogger<CachedAiExplanationProvider>.Instance);

        var response = await service.ExplainAsync(CreateRequest());

        Assert.Equal("Generated explanation", response.Explanation);
        Assert.Equal(1, provider.CallCount);
        Assert.Single(cache.Items);
        Assert.Equal("Generated explanation", cache.Items[0].Explanation);
    }

    [Fact]
    public async Task ExplainAsync_CacheUnavailableStillReturnsProviderResult()
    {
        var provider = new FakeGeminiProvider();
        var service = new CachedAiExplanationProvider(
            provider,
            new FakeCacheRepository(shouldFail: true),
            NullLogger<CachedAiExplanationProvider>.Instance);

        var response = await service.ExplainAsync(CreateRequest());

        Assert.Equal("Generated explanation", response.Explanation);
        Assert.Equal(1, provider.CallCount);
    }

    [Fact]
    public async Task ExplainAsync_ProviderFailureReturnsEngineFallback()
    {
        var service = new CachedAiExplanationProvider(
            new FakeGeminiProvider(shouldFail: true),
            new FakeCacheRepository(),
            NullLogger<CachedAiExplanationProvider>.Instance);

        var response = await service.ExplainAsync(CreateRequest());

        Assert.StartsWith("AI explanation is unavailable right now. Engine summary:", response.Explanation);
        Assert.Contains("Armor scaling helps", response.Explanation);
        Assert.Contains("Play short trades.", response.Explanation);
    }

    [Fact]
    public async Task ExplainAsync_RateLimitReturnsRateLimitFallback()
    {
        var service = new CachedAiExplanationProvider(
            new FakeGeminiProvider(rateLimited: true),
            new FakeCacheRepository(),
            NullLogger<CachedAiExplanationProvider>.Instance);

        var response = await service.ExplainAsync(CreateRequest());

        Assert.Equal(CachedAiExplanationProvider.RateLimitFallback, response.Explanation);
    }

    [Fact]
    public void BuildCacheKey_SameEnemyTeamInDifferentOrderReturnsSameKey()
    {
        var first = CreateRequest();
        var second = CreateRequest();
        second.EnemyTeam = new List<string> { "Olaf", "Senna", "Jhin" };

        Assert.Equal(
            AiExplanationCacheKeyBuilder.Build(first),
            AiExplanationCacheKeyBuilder.Build(second));
    }

    private static AiExplanationRequestDto CreateRequest()
    {
        return new AiExplanationRequestDto
        {
            Role = "Baron",
            LaneEnemy = "Darius",
            EnemyTeam = new List<string> { "Senna", "Jhin", "Olaf" },
            Champion = "Malphite",
            Score = 84,
            Reasons = new List<string> { "Armor scaling helps against physical damage" },
            Plan = "Play short trades."
        };
    }

    private sealed class FakeCacheRepository : IAiExplanationCacheRepository
    {
        private readonly bool _shouldFail;

        public FakeCacheRepository(bool shouldFail = false)
        {
            _shouldFail = shouldFail;
        }

        public List<AiExplanationCache> Items { get; } = new();

        public Task<AiExplanationCache?> GetByCacheKeyAsync(
            string cacheKey,
            CancellationToken cancellationToken = default)
        {
            if (_shouldFail)
            {
                throw new InvalidOperationException("Cache table unavailable.");
            }

            return Task.FromResult(Items.SingleOrDefault(item => item.CacheKey == cacheKey));
        }

        public Task SaveAsync(
            AiExplanationCache cache,
            CancellationToken cancellationToken = default)
        {
            if (_shouldFail)
            {
                throw new InvalidOperationException("Cache table unavailable.");
            }

            Items.Add(cache);
            return Task.CompletedTask;
        }
    }

    private sealed class FakeGeminiProvider : IExternalAiExplanationProvider
    {
        private readonly bool _shouldFail;
        private readonly bool _rateLimited;

        public FakeGeminiProvider(bool shouldFail = false, bool rateLimited = false)
        {
            _shouldFail = shouldFail;
            _rateLimited = rateLimited;
        }

        public int CallCount { get; private set; }

        public Task<AiExplanationResponseDto> ExplainAsync(AiExplanationRequestDto request, CancellationToken cancellationToken = default)
        {
            CallCount++;

            if (_rateLimited)
            {
                throw new AiProviderRateLimitException(
                    "Provider rate limit reached.",
                    new InvalidOperationException("429 quota exceeded"));
            }

            if (_shouldFail)
            {
                throw new InvalidOperationException("Provider unavailable.");
            }

            return Task.FromResult(new AiExplanationResponseDto
            {
                Explanation = "Generated explanation"
            });
        }

        public Task<IReadOnlyDictionary<string, string>> ExplainBatchAsync(
            IReadOnlyCollection<AiExplanationRequestDto> requests,
            CancellationToken cancellationToken = default)
        {
            throw new NotSupportedException();
        }
    }
}
