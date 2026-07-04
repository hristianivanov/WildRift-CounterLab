using System.Net.Http.Json;

using Microsoft.Extensions.Logging;

using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Interfaces;
using WildRiftCounterLab.Domain.Entities;
using WildRiftCounterLab.Infrastructure.ExternalApis.DataDragon;

namespace WildRiftCounterLab.Infrastructure.Services;

public sealed class ChampionSyncService : IChampionSyncService
{
    // Tags Data Dragon uses → our AllowedTags equivalents
    private static readonly Dictionary<string, string> TagMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Fighter"] = "fighter",
        ["Tank"] = "tank",
        ["Mage"] = "mage",
        ["Assassin"] = "assassin",
        ["Marksman"] = "marksman",
        ["Support"] = "support",
    };

    private readonly IChampionRepository _champions;
    private readonly HttpClient _http;
    private readonly ILogger<ChampionSyncService> _logger;

    public ChampionSyncService(
        IChampionRepository champions,
        IHttpClientFactory httpClientFactory,
        ILogger<ChampionSyncService> logger)
    {
        _champions = champions;
        _http = httpClientFactory.CreateClient("DataDragon");
        _logger = logger;
    }

    public async Task<ChampionSyncResultDto> SyncFromDataDragonAsync(
        CancellationToken cancellationToken = default)
    {
        var version = await FetchLatestVersionAsync(cancellationToken);
        _logger.LogInformation("Syncing champions from Data Dragon version {Version}", version);

        var response = await _http.GetFromJsonAsync<DataDragonChampionResponse>(
            $"https://ddragon.leagueoflegends.com/cdn/{version}/data/en_US/champion.json",
            cancellationToken);

        if (response is null)
        {
            throw new InvalidOperationException("Data Dragon returned an empty champion list.");
        }

        var existing = await _champions.GetAllAsync(cancellationToken);
        var existingNames = existing
            .Select(c => c.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var toAdd = new List<Champion>();
        var added = new List<string>();
        var skipped = 0;

        foreach (var entry in response.Data.Values)
        {
            if (existingNames.Contains(entry.Name))
            {
                skipped++;
                continue;
            }

            var tags = entry.Tags
                .Where(t => TagMap.ContainsKey(t))
                .Select(t => TagMap[t])
                .ToList();

            toAdd.Add(new Champion { Name = entry.Name, Roles = [], Tags = tags });
            added.Add(entry.Name);
        }

        if (toAdd.Count > 0)
        {
            await _champions.AddRangeAsync(toAdd, cancellationToken);
        }

        _logger.LogInformation(
            "Data Dragon sync complete: {Added} added, {Skipped} already present",
            added.Count, skipped);

        return new ChampionSyncResultDto
        {
            Added = added.Count,
            Skipped = skipped,
            AddedNames = added,
        };
    }

    private async Task<string> FetchLatestVersionAsync(CancellationToken cancellationToken)
    {
        var versions = await _http.GetFromJsonAsync<List<string>>(
            "https://ddragon.leagueoflegends.com/api/versions.json",
            cancellationToken);

        if (versions is null || versions.Count == 0)
        {
            throw new InvalidOperationException("Could not retrieve versions from Data Dragon.");
        }

        return versions[0];
    }
}
