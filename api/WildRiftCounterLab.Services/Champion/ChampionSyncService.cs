namespace WildRiftCounterLab.Services;

using Microsoft.Extensions.Logging;

using WildRiftCounterLab.Contracts;
using WildRiftCounterLab.Data.Models;
using WildRiftCounterLab.Services.Models;

public sealed class ChampionSyncService : IChampionSyncService
{
    private readonly IChampionRepository _champions;
    private readonly IDataDragonClient _dataDragon;
    private readonly ILogger<ChampionSyncService> _logger;

    public ChampionSyncService(
        IChampionRepository champions,
        IDataDragonClient dataDragon,
        ILogger<ChampionSyncService> logger)
    {
        _champions = champions;
        _dataDragon = dataDragon;
        _logger = logger;
    }

    public async Task<ChampionSyncResultDto> SyncFromDataDragonAsync(
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Syncing Wild Rift champion roster from Community Dragon");

        var roster = await _dataDragon.FetchWildRiftRosterAsync(cancellationToken);

        var existing = await _champions.GetAllAsync(cancellationToken);
        var existingByName = existing.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

        // Remove DB entries that are no longer in the WR roster and have no custom roles set
        var toRemove = existing
            .Where(c => !roster.ContainsKey(c.Name) && c.Roles.Count == 0)
            .ToList();

        if (toRemove.Count > 0)
        {
            await _champions.DeleteRangeAsync(toRemove, cancellationToken);
            foreach (var c in toRemove) existingByName.Remove(c.Name);
            _logger.LogInformation("Removed {Count} champions no longer in the Wild Rift roster", toRemove.Count);
        }

        var toAdd = new List<Champion>();
        var added = new List<string>();
        var skipped = 0;

        foreach (var (name, cdRoles) in roster)
        {
            if (existingByName.ContainsKey(name))
            {
                skipped++;
                continue;
            }

            // CDragon already provides lowercase class roles: fighter, mage, tank, assassin, marksman, support
            var tags = cdRoles.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();

            toAdd.Add(new Champion { Name = name, Roles = [], Tags = tags });
            added.Add(name);
        }

        if (toAdd.Count > 0)
            await _champions.AddRangeAsync(toAdd, cancellationToken);

        _logger.LogInformation(
            "Wild Rift roster sync complete: {Added} added, {Removed} removed, {Skipped} already present",
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
}
