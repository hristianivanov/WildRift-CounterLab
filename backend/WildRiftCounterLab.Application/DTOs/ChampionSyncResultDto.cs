namespace WildRiftCounterLab.Application.DTOs;

public sealed class ChampionSyncResultDto
{
    public int Added { get; init; }
    public int Skipped { get; init; }
    public IReadOnlyList<string> AddedNames { get; init; } = [];
}
