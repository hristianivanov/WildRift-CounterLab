namespace WildRiftCounterLab.Services.Models;

public sealed class PatchCheckResultDto
{
    public string LatestVersion { get; init; } = string.Empty;
    public string? PreviousVersion { get; init; }
    public bool SyncTriggered { get; init; }
    public ChampionSyncResultDto? SyncResult { get; init; }
}
