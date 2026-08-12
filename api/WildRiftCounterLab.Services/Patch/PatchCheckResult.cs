namespace WildRiftCounterLab.Services.Patch;

using WildRiftCounterLab.Services.Models;

public sealed class PatchCheckResult
{
    public string LatestVersion { get; init; } = string.Empty;
    public string? PreviousVersion { get; init; }
    public bool SyncTriggered { get; init; }
    public ChampionSyncResultDto? SyncResult { get; init; }
}
