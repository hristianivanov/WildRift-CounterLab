namespace WildRiftCounterLab.Services.Patch;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using WildRiftCounterLab.Contracts;

public sealed class PatchMonitorService : BackgroundService
{
    public const string LastSyncedVersionKey = "PatchMonitor:LastSyncedVersion";

    private static readonly TimeSpan CheckInterval = TimeSpan.FromHours(24);
    private static readonly TimeSpan StartupDelay = TimeSpan.FromSeconds(30);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<PatchMonitorService> _logger;

    public PatchMonitorService(IServiceScopeFactory scopeFactory, ILogger<PatchMonitorService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Delay(StartupDelay, stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndSyncAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Patch monitor: unhandled error during patch check");
            }

            await Task.Delay(CheckInterval, stoppingToken);
        }
    }

    public async Task<PatchCheckResult> CheckAndSyncAsync(CancellationToken cancellationToken = default)
    {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var dataDragon = scope.ServiceProvider.GetRequiredService<IDataDragonClient>();
        var settings = scope.ServiceProvider.GetRequiredService<IAppSettingRepository>();
        var syncService = scope.ServiceProvider.GetRequiredService<IChampionSyncService>();

        var latestVersion = await dataDragon.FetchLatestVersionAsync(cancellationToken);
        var lastSynced = await settings.GetAsync(LastSyncedVersionKey, cancellationToken);

        if (latestVersion == lastSynced)
        {
            _logger.LogDebug("Patch monitor: already on {Version}, no sync needed", latestVersion);
            return new PatchCheckResult { LatestVersion = latestVersion, PreviousVersion = lastSynced, SyncTriggered = false };
        }

        _logger.LogInformation(
            "Patch monitor: new Data Dragon version detected ({Previous} → {Latest}), starting champion sync",
            lastSynced ?? "none", latestVersion);

        var syncResult = await syncService.SyncFromDataDragonAsync(cancellationToken);
        await settings.SetAsync(LastSyncedVersionKey, latestVersion, cancellationToken);

        _logger.LogInformation(
            "Patch monitor: sync complete for {Version} — {Added} added, {Removed} removed",
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
