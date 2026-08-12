namespace WildRiftCounterLab.Services;

using Microsoft.Extensions.Logging;

using WildRiftCounterLab.Contracts;
using WildRiftCounterLab.Data.Models;
using WildRiftCounterLab.Services.Models;

public sealed class ChampionSyncService : IChampionSyncService
{
    // Source of truth for which champions are available in Wild Rift.
    // CDragon PC LoL data includes all of these; we filter down to this set.
    // Add new names here when a champion releases on WR.
    private static readonly HashSet<string> WildRiftRoster = new(StringComparer.OrdinalIgnoreCase)
    {
        "Aatrox", "Ahri", "Akali", "Akshan", "Alistar", "Ambessa", "Amumu",
        "Annie", "Ashe", "Aurelion Sol", "Aurora",
        "Bard", "Blitzcrank", "Brand", "Braum",
        "Caitlyn", "Camille", "Cho'Gath", "Corki",
        "Darius", "Diana", "Dr. Mundo", "Draven",
        "Ekko", "Evelynn", "Ezreal",
        "Fiddlesticks", "Fiora", "Fizz",
        "Galio", "Garen", "Gnar", "Gragas", "Graves", "Gwen",
        "Hecarim", "Heimerdinger",
        "Irelia",
        "Janna", "Jarvan IV", "Jax", "Jayce", "Jhin", "Jinx",
        "K'Sante", "Kai'Sa", "Kalista", "Karma", "Kassadin", "Katarina", "Kayle", "Kayn", "Kennen", "Kha'Zix", "Kindred", "Kog'Maw",
        "Lee Sin", "Leona", "Lillia", "Lissandra", "Lucian", "Lulu", "Lux",
        "Malphite", "Maokai", "Master Yi", "Mel", "Milio", "Miss Fortune", "Mordekaiser", "Morgana",
        "Nami", "Nasus", "Nautilus", "Nidalee", "Nilah", "Nocturne", "Norra", "Nunu & Willump",
        "Olaf", "Orianna", "Ornn",
        "Pantheon", "Poppy", "Pyke",
        "Rakan", "Rammus", "Rell", "Renekton", "Rengar", "Riven", "Rumble", "Ryze",
        "Samira", "Senna", "Seraphine", "Sett", "Shen", "Shyvana", "Singed", "Sion", "Sivir", "Skarner", "Smolder", "Sona", "Soraka", "Swain", "Syndra",
        "Taliyah", "Talon", "Teemo", "Thresh", "Tristana", "Tryndamere", "Twisted Fate", "Twitch",
        "Urgot",
        "Varus", "Vayne", "Veigar", "Vel'Koz", "Vex", "Vi", "Viego", "Viktor", "Vladimir", "Volibear",
        "Warwick", "Wukong",
        "Xayah", "Xin Zhao",
        "Yasuo", "Yone", "Yunara", "Yuumi",
        "Zed", "Zeri", "Ziggs", "Zilean", "Zoe", "Zyra",
    };

    // Champions that are WR-exclusive and not present in PC LoL CDragon data.
    // They are added to the roster with no class tags.
    private static readonly HashSet<string> WrExclusiveChampions = new(StringComparer.OrdinalIgnoreCase)
    {
        "Norra",
    };

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

        var cdData = await _dataDragon.FetchWildRiftRosterAsync(cancellationToken);

        var existing = await _champions.GetAllAsync(cancellationToken);
        var existingByName = existing.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

        var toRemove = existing
            .Where(c => !WildRiftRoster.Contains(c.Name) && c.Roles.Count == 0)
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

        foreach (var name in WildRiftRoster)
        {
            if (existingByName.ContainsKey(name))
            {
                skipped++;
                continue;
            }

            var tags = WrExclusiveChampions.Contains(name)
                ? new List<string>()
                : cdData.TryGetValue(name, out var cdRoles)
                    ? cdRoles.Where(r => !string.IsNullOrWhiteSpace(r)).ToList()
                    : new List<string>();

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
