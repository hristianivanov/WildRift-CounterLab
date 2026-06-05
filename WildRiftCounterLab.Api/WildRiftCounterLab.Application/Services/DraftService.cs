using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Engine;
using WildRiftCounterLab.Application.Interfaces;
using WildRiftCounterLab.Domain.Constants;

namespace WildRiftCounterLab.Application.Services;

public class DraftService
{
    private readonly ScoreEngine _scoreEngine;
    private readonly ReasonEngine _reasonEngine;
    private readonly PlanEngine _planEngine;

    private readonly IChampionRepository _championRepository;
    private readonly IMatchupRuleRepository _matchupRuleRepository;

    public DraftService(
        ScoreEngine scoreEngine,
        ReasonEngine reasonEngine,
        PlanEngine planEngine,
        IChampionRepository championRepository,
        IMatchupRuleRepository matchupRuleRepository)
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
        if (!AllowedRoles.Values.Contains(request.Role))
        {
            throw new ArgumentException("Invalid role.");
        }

        if (string.IsNullOrWhiteSpace(request.LaneEnemy))
        {
            throw new ArgumentException("Lane enemy is required.");
        }

        if (request.EnemyTeam.Count > 5)
        {
            throw new ArgumentException("Enemy team cannot have more than 5 champions.");
        }

        if (request.EnemyTeam.Distinct().Count() != request.EnemyTeam.Count)
        {
            throw new ArgumentException("Enemy team cannot contain duplicate champions.");
        }

        if (request.EnemyTeam.Contains(request.LaneEnemy))
        {
            throw new ArgumentException("Lane enemy cannot also be inside enemy team.");
        }


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