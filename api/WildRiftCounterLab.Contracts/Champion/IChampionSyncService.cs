using WildRiftCounterLab.Services.Models;

namespace WildRiftCounterLab.Contracts;

public interface IChampionSyncService
{
    Task<ChampionSyncResultDto> SyncFromDataDragonAsync(CancellationToken cancellationToken = default);
}
