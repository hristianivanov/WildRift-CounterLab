using WildRiftCounterLab.Services.Models;

namespace WildRiftCounterLab.Services.Interfaces;

public interface IChampionSyncService
{
    Task<ChampionSyncResultDto> SyncFromDataDragonAsync(CancellationToken cancellationToken = default);
}
