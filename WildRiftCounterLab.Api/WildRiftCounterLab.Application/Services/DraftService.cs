using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Engine;
using WildRiftCounterLab.Application.Interfaces;

namespace WildRiftCounterLab.Application.Services;

public class DraftService
{
    private readonly ScoreEngine _scoreEngine;
    private readonly ReasonEngine _reasonEngine;
    private readonly IChampionRepository _championRepository;
    private readonly IMatchupRuleRepository _matchupRuleRepository;
    private readonly PlanEngine _planEngine;

    public DraftService(
        ScoreEngine scoreEngine,
        ReasonEngine reasonEngine,
        IChampionRepository championRepository,
        IMatchupRuleRepository matchupRuleRepository,
        PlanEngine planEngine)
    {
        _scoreEngine = scoreEngine;
        _reasonEngine = reasonEngine;
        _championRepository = championRepository;
        _matchupRuleRepository = matchupRuleRepository;
        _planEngine = planEngine;
    }

    public async Task<DraftRecommendationResponseDto> GetRecommendations(
        DraftRecommendationRequestDto request)
    {
        var enemies = new List<string>();

        if (!string.IsNullOrWhiteSpace(request.LaneEnemy))
        {
            enemies.Add(request.LaneEnemy);
        }

        enemies.AddRange(request.EnemyTeam);

        var rules = await _matchupRuleRepository.GetRulesForDraftAsync(
            request.Role,
            enemies);

        var candidateChampions = await _championRepository.GetAllAsync();

        candidateChampions = candidateChampions
            .Where(champion => champion.Roles.Contains(request.Role))
            .ToList();

        var recommendations = candidateChampions
            .Select(champion => new DraftRecommendationDto
            {
                Champion = champion.Name,
                Score = _scoreEngine.CalculateScore(champion.Name, rules),
                Reasons = _reasonEngine.BuildReasons(champion.Name, rules),
                Plan = _planEngine.BuildPlan(champion.Name, rules)
            })
            .OrderByDescending(x => x.Score)
            .Take(5)
            .ToList();

        return new DraftRecommendationResponseDto
        {
            Role = request.Role,
            LaneEnemy = request.LaneEnemy,
            Recommendations = recommendations
        };
    }
}