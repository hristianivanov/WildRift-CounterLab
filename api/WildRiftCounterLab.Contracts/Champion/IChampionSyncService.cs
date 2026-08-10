namespace WildRiftCounterLab.Contracts;

using WildRiftCounterLab.Services.Models;

public interface IChampionSyncService
{
    Task<ChampionSyncResultDto> SyncFromDataDragonAsync(CancellationToken cancellationToken = default);
}