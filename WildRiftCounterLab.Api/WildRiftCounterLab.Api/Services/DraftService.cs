namespace WildRiftCounterLab.Api.Services;

using Data;

using DTOs;

using Engine;

using WildRiftCounterLab.Api.Repositories;

public class DraftService
{
    private readonly ScoreEngine _scoreEngine;
    private readonly ReasonEngine _reasonEngine;
    private readonly ChampionRepository _championRepository;
    private readonly PlanEngine _planEngine;

    public DraftService(
        ScoreEngine scoreEngine,
        ReasonEngine reasonEngine,
        ChampionRepository championRepository,
        PlanEngine planEngine)
    {
        _scoreEngine = scoreEngine;
        _reasonEngine = reasonEngine;
        _championRepository = championRepository;
        _planEngine = planEngine;
    }

    public async Task<List<DraftRecommendationDto>> GetRecommendations(DraftRequestDto request)
    {
        var candidateChampions = await _championRepository
            .GetAllAsync();

        candidateChampions = candidateChampions
            .Where(champion => champion.Roles.Contains(request.Role))
            .ToList();

        return candidateChampions
            .Select(champion => new DraftRecommendationDto
            {
                Champion = champion.Name,
                Score = _scoreEngine.CalculateScore(champion.Name, request),
                Reasons = _reasonEngine.BuildReasons(champion.Name, request),
                Plan = _planEngine.BuildPlan(champion.Name, request)
            })
            .OrderByDescending(x => x.Score)
            .ToList();
    }
}