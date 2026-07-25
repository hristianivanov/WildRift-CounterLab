using WildRiftCounterLab.Application.DTOs;

namespace WildRiftCounterLab.Application.Interfaces;

public interface IChampionSyncService
{
    Task<ChampionSyncResultDto> SyncFromDataDragonAsync(CancellationToken cancellationToken = default);
}
