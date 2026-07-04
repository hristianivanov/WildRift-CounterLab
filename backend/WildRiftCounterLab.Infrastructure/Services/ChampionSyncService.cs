using System.Net.Http.Json;

using Microsoft.Extensions.Logging;

using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Interfaces;
using WildRiftCounterLab.Domain.Entities;
using WildRiftCounterLab.Infrastructure.ExternalApis.DataDragon;

namespace WildRiftCounterLab.Infrastructure.Services;

public sealed class ChampionSyncService : IChampionSyncService
{
    // Confirmed Wild Rift champion pool — update this list when new champions release on WR.
    // Names must match Data Dragon's "name" field exactly.
    private static readonly HashSet<string> WildRiftRoster = new(StringComparer.OrdinalIgnoreCase)
    {
        "Ahri", "Akali", "Akshan", "Alistar", "Ambessa", "Amumu",
        "Annie", "Ashe", "Aurelion Sol", "Blitzcrank", "Braum",
        "Caitlyn", "Camille", "Corki", "Darius", "Diana", "Dr. Mundo",
        "Draven", "Ekko", "Evelynn", "Ezreal", "Fiora", "Fizz",
        "Galio", "Garen", "Gragas", "Graves", "Gwen",
        "Irelia", "Janna", "Jarvan IV", "Jax", "Jayce", "Jhin",
        "Jinx", "Kai'Sa", "Karma", "Katarina", "Kayle", "Kennen",
        "Kha'Zix", "Lee Sin", "Leona", "Lissandra", "Lucian", "Lulu",
        "Lux", "Malphite", "Master Yi", "Miss Fortune", "Morgana",
        "Nami", "Nasus", "Nautilus", "Nunu & Willump", "Olaf",
        "Orianna", "Ornn", "Pantheon", "Pyke", "Rammus", "Rakan",
        "Renekton", "Riven", "Samira", "Senna", "Seraphine", "Sett",
        "Shen", "Shyvana", "Singed", "Sona", "Soraka", "Teemo",
        "Thresh", "Tristana", "Twisted Fate", "Varus", "Vayne", "Vex",
        "Vi", "Warwick", "Wukong", "Xayah", "Xin Zhao", "Yasuo",
        "Yone", "Yuumi", "Zed", "Ziggs", "Zoe", "Zyra",
    };

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
        var existingByName = existing.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

        // Remove champions not in WR that have no roles (safe to delete — no roles means never
        // manually configured, so these were incorrectly synced entries).
        var toRemove = existing
            .Where(c => !WildRiftRoster.Contains(c.Name) && c.Roles.Count == 0)
            .ToList();

        if (toRemove.Count > 0)
        {
            await _champions.DeleteRangeAsync(toRemove, cancellationToken);
            foreach (var c in toRemove) existingByName.Remove(c.Name);
            _logger.LogInformation("Removed {Count} non-WR champions with no roles", toRemove.Count);
        }

        // Add WR champions missing from DB using Data Dragon for tags
        var ddByName = response.Data.Values
            .ToDictionary(e => e.Name, StringComparer.OrdinalIgnoreCase);

        var toAdd = new List<Champion>();
        var added = new List<string>();
        var skipped = 0;

        foreach (var name in WildRiftRoster)
        {
            if (existingByName.ContainsKey(name))
            {
                skipped++;
                continue;
            }

            var tags = ddByName.TryGetValue(name, out var entry)
                ? entry.Tags.Where(t => TagMap.ContainsKey(t)).Select(t => TagMap[t]).ToList()
                : new List<string>();

            toAdd.Add(new Champion { Name = name, Roles = [], Tags = tags });
            added.Add(name);
        }

        if (toAdd.Count > 0)
        {
            await _champions.AddRangeAsync(toAdd, cancellationToken);
        }

        _logger.LogInformation(
            "Data Dragon sync complete: {Added} added, {Removed} removed, {Skipped} already present",
            added.Count, toRemove.Count, skipped);

        return new ChampionSyncResultDto
        {
            Added = added.Count,
            Removed = toRemove.Count,
            Skipped = skipped,
            AddedNames = added,
            RemovedNames = toRemove.Select(c => c.Name).ToList(),
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
