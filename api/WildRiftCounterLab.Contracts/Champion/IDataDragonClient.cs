namespace WildRiftCounterLab.Contracts;

public interface IDataDragonClient
{
    Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> FetchChampionTagsAsync(CancellationToken cancellationToken = default);
}