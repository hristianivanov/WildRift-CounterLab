namespace WildRiftCounterLab.Contracts;

public interface IDataDragonClient
{
    Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> FetchChampionTagsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Fetches the live Wild Rift champion roster from Community Dragon.
    /// Returns champion name → list of class roles (fighter, mage, tank, assassin, marksman, support).
    /// </summary>
    Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> FetchWildRiftRosterAsync(CancellationToken cancellationToken = default);
}