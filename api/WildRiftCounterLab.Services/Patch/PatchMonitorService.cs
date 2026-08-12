namespace WildRiftCounterLab.Services.Patch;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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
                await using var scope = _scopeFactory.CreateAsyncScope();
                var checker = scope.ServiceProvider.GetRequiredService<PatchCheckService>();
                await checker.CheckAndSyncAsync(stoppingToken);
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
}
