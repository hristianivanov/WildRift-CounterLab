namespace WildRiftCounterLab.Services;

using Mapster;

using WildRiftCounterLab.Common;
using WildRiftCounterLab.Contracts;
using WildRiftCounterLab.Data.Models;
using WildRiftCounterLab.Services.Models;

public class MatchupTipAdminService
{
    private readonly IMatchupTipRepository _matchupTipRepository;
    private readonly IChampionRepository _championRepository;

    public MatchupTipAdminService(
        IMatchupTipRepository matchupTipRepository,
        IChampionRepository championRepository)
    {
        _matchupTipRepository = matchupTipRepository;
        _championRepository = championRepository;
    }

    public async Task<List<MatchupTipDto>> GetAllAsync()
    {
        var tips = await _matchupTipRepository.GetAllAsync();

        return tips.Adapt<List<MatchupTipDto>>();
    }

    public async Task<MatchupTipDto> GetByIdAsync(int id)
    {
        var tip = await GetRequiredTipAsync(id);

        return tip.Adapt<MatchupTipDto>();
    }

    public async Task<MatchupTipDto> CreateAsync(CreateMatchupTipRequestDto request)
    {
        var (champion, enemyChampion) = await ValidateChampionsAsync(request.Champion, request.EnemyChampion);

        if (await _matchupTipRepository.ExistsAsync(champion, enemyChampion, request.Tip.Trim()))
        {
            throw new ArgumentException("An identical tip already exists for this matchup.");
        }

        var tip = new MatchupTip
        {
            Champion = champion,
            EnemyChampion = enemyChampion,
            Tip = request.Tip.Trim(),
            AbilityTag = request.AbilityTag?.Trim(),
        };

        await _matchupTipRepository.AddAsync(tip);

        return tip.Adapt<MatchupTipDto>();
    }

    public async Task<MatchupTipDto> UpdateAsync(int id, UpdateMatchupTipRequestDto request)
    {
        var tip = await GetRequiredTipAsync(id);
        var (champion, enemyChampion) = await ValidateChampionsAsync(request.Champion, request.EnemyChampion);

        tip.Champion = champion;
        tip.EnemyChampion = enemyChampion;
        tip.Tip = request.Tip.Trim();
        tip.AbilityTag = request.AbilityTag?.Trim();

        await _matchupTipRepository.UpdateAsync(tip);

        return tip.Adapt<MatchupTipDto>();
    }

    public async Task DeleteAsync(int id)
    {
        var tip = await GetRequiredTipAsync(id);

        await _matchupTipRepository.DeleteAsync(tip);
    }

    private async Task<MatchupTip> GetRequiredTipAsync(int id)
    {
        return await _matchupTipRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Matchup tip with id {id} was not found.");
    }

    private async Task<(string Champion, string EnemyChampion)> ValidateChampionsAsync(
        string champion,
        string enemyChampion)
    {
        var champions = await _championRepository.GetAllAsync();
        var byName = champions.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

        if (!byName.TryGetValue(champion, out var canonicalChampion))
        {
            throw new ArgumentException($"Unknown champion: {champion}.");
        }

        if (!byName.TryGetValue(enemyChampion, out var canonicalEnemy))
        {
            throw new ArgumentException($"Unknown champion: {enemyChampion}.");
        }

        return (canonicalChampion.Name, canonicalEnemy.Name);
    }
}
