using WildRiftCounterLab.Api.Data;
using WildRiftCounterLab.Api.DTOs;
using WildRiftCounterLab.Api.Engine;

namespace WildRiftCounterLab.Api.Services;

public class DraftService
{
    private readonly ScoreEngine _scoreEngine;
    private readonly ReasonEngine _reasonEngine;
    private readonly ChampionDataProvider _championDataProvider;
    private readonly PlanEngine _planEngine;

    public DraftService(
        ScoreEngine scoreEngine,
        ReasonEngine reasonEngine,
        ChampionDataProvider championDataProvider,
        PlanEngine planEngine)
    {
        _scoreEngine = scoreEngine;
        _reasonEngine = reasonEngine;
        _championDataProvider = championDataProvider;
        _planEngine = planEngine;
    }

    public List<DraftRecommendationDto> GetRecommendations(DraftRequestDto request)
    {
        var candidateChampions = _championDataProvider
            .GetChampions()
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