using System.Text.Json.Serialization;

namespace WildRiftCounterLab.Infrastructure.ExternalApis.DataDragon;

internal sealed class DataDragonChampionResponse
{
    [JsonPropertyName("data")]
    public Dictionary<string, DataDragonChampionEntry> Data { get; init; } = new();
}

internal sealed class DataDragonChampionEntry
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("tags")]
    public List<string> Tags { get; init; } = [];
}
