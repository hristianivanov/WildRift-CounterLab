namespace WildRiftCounterLab.Services.Patch;

using Microsoft.Extensions.Logging;

using WildRiftCounterLab.Contracts;

public sealed class PatchCheckService
{
    private readonly IDataDragonClient _dataDragon;
    private readonly IAppSettingRepository _settings;
    private readonly IChampionSyncService _syncService;
    private readonly IAiExplanationCacheRepository _aiCache;
    private readonly ILogger<PatchCheckService> _logger;

    public PatchCheckService(
        IDataDragonClient dataDragon,
        IAppSettingRepository settings,
        IChampionSyncService syncService,
        IAiExplanationCacheRepository aiCache,
        ILogger<PatchCheckService> logger)
    {
        _dataDragon = dataDragon;
        _settings = settings;
        _syncService = syncService;
        _aiCache = aiCache;
        _logger = logger;
    }

    public async Task<PatchCheckResult> CheckAndSyncAsync(CancellationToken cancellationToken = default)
    {
        var latestVersion = await _dataDragon.FetchLatestVersionAsync(cancellationToken);
        var lastSynced = await _settings.GetAsync(PatchMonitorService.LastSyncedVersionKey, cancellationToken);

        if (latestVersion == lastSynced)
        {
            _logger.LogDebug("Patch check: already on {Version}, no sync needed", latestVersion);
            return new PatchCheckResult { LatestVersion = latestVersion, PreviousVersion = lastSynced, SyncTriggered = false };
        }

        _logger.LogInformation(
            "Patch check: new Data Dragon version detected ({Previous} → {Latest}), starting champion sync",
            lastSynced ?? "none", latestVersion);

        var syncResult = await _syncService.SyncFromDataDragonAsync(cancellationToken);
        await _aiCache.InvalidateAllAsync(cancellationToken);
        await _settings.SetAsync(PatchMonitorService.LastSyncedVersionKey, latestVersion, cancellationToken);

        _logger.LogInformation(
            "Patch check: sync complete for {Version} — {Added} added, {Removed} removed",
            latestVersion, syncResult.Added, syncResult.Removed);

        return new PatchCheckResult
        {
            LatestVersion = latestVersion,
            PreviousVersion = lastSynced,
            SyncTriggered = true,
            SyncResult = syncResult,
        };
    }
}
