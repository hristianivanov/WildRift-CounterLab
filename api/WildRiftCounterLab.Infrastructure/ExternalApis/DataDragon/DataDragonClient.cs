namespace WildRiftCounterLab.Infrastructure.ExternalApis.DataDragon;

using System.Net.Http.Json;

using WildRiftCounterLab.Contracts;

public sealed class DataDragonClient : IDataDragonClient
{
    private readonly HttpClient _http;

    public DataDragonClient(IHttpClientFactory httpClientFactory)
    {
        _http = httpClientFactory.CreateClient("DataDragon");
    }

    public async Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> FetchChampionTagsAsync(
        CancellationToken cancellationToken = default)
    {
        var version = await FetchLatestVersionAsync(cancellationToken);

        var response = await _http.GetFromJsonAsync<DataDragonChampionResponse>(
            $"https://ddragon.leagueoflegends.com/cdn/{version}/data/en_US/champion.json",
            cancellationToken);

        if (response is null)
            throw new InvalidOperationException("Data Dragon returned an empty champion list.");

        return response.Data.Values.ToDictionary(
            entry => entry.Name,
            entry => (IReadOnlyList<string>)entry.Tags,
            StringComparer.OrdinalIgnoreCase);
    }

    public async Task<IReadOnlyDictionary<string, IReadOnlyList<string>>> FetchWildRiftRosterAsync(
        CancellationToken cancellationToken = default)
    {
        // The /wildrift/ CDragon path is defunct. PC LoL CDragon (/latest/) includes all
        // WR champions (including WR-first releases like Yunara and Mel) and is kept current.
        var champions = await _http.GetFromJsonAsync<List<CdChampionSummary>>(
            "https://raw.communitydragon.org/latest/plugins/rcp-be-lol-game-data/global/default/v1/champion-summary.json",
            cancellationToken);

        if (champions is null)
            throw new InvalidOperationException("Community Dragon returned an empty champion list.");

        return champions
            .Where(c => c.Id > 0 && !string.IsNullOrWhiteSpace(c.Name))
            .ToDictionary(
                c => c.Name,
                c => (IReadOnlyList<string>)c.Roles,
                StringComparer.OrdinalIgnoreCase);
    }

    private async Task<string> FetchLatestVersionAsync(CancellationToken cancellationToken)
    {
        var versions = await _http.GetFromJsonAsync<List<string>>(
            "https://ddragon.leagueoflegends.com/api/versions.json",
            cancellationToken);

        if (versions is null || versions.Count == 0)
            throw new InvalidOperationException("Could not retrieve versions from Data Dragon.");

        return versions[0];
    }

    private sealed class CdChampionSummary
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}