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
    private readonly MatchupRuleRepository _matchupRuleRepository;
    private readonly PlanEngine _planEngine;

    public DraftService(
        ScoreEngine scoreEngine,
        ReasonEngine reasonEngine,
        ChampionRepository championRepository,
        MatchupRuleRepository matchupRuleRepository,
        PlanEngine planEngine)
    {
        _scoreEngine = scoreEngine;
        _reasonEngine = reasonEngine;
        _championRepository = championRepository;
        _matchupRuleRepository = matchupRuleRepository;
        _planEngine = planEngine;
    }

    public async Task<List<DraftRecommendationDto>> GetRecommendations(DraftRequestDto request)
    {
        var candidateChampions = await _championRepository
            .GetAllAsync();

        candidateChampions = candidateChampions
            .Where(champion => champion.Roles.Contains(request.Role))
            .ToList();

        var enemies = new List<string>();

        if (!string.IsNullOrWhiteSpace(request.LaneEnemy))
        {
            enemies.Add(request.LaneEnemy);
        }

        enemies.AddRange(request.EnemyTeam);

        var rules = await _matchupRuleRepository.GetRulesForDraftAsync(request.Role, enemies);

        return candidateChampions
            .Select(champion => new DraftRecommendationDto
            {
                Champion = champion.Name,
                Score = _scoreEngine.CalculateScore(champion.Name, rules),
                Reasons = _reasonEngine.BuildReasons(champion.Name, request),
                Plan = _planEngine.BuildPlan(champion.Name, request)
            })
            .OrderByDescending(x => x.Score)
            .ToList();
    }
}