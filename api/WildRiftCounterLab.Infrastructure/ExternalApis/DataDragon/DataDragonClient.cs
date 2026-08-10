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

    private async Task<string> FetchLatestVersionAsync(CancellationToken cancellationToken)
    {
        var versions = await _http.GetFromJsonAsync<List<string>>(
            "https://ddragon.leagueoflegends.com/api/versions.json",
            cancellationToken);

        if (versions is null || versions.Count == 0)
            throw new InvalidOperationException("Could not retrieve versions from Data Dragon.");

        return versions[0];
    }
}