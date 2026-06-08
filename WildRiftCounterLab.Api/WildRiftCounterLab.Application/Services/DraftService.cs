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
        var role = AllowedRoles.Values
            .SingleOrDefault(allowedRole =>
                allowedRole.Equals(request.Role, StringComparison.OrdinalIgnoreCase));

        if (role is null)
        {
            throw new ArgumentException("Invalid role.");
        }

        if (string.IsNullOrWhiteSpace(request.LaneEnemy))
        {
            throw new ArgumentException("Lane enemy is required.");
        }

        if (request.EnemyTeam.Count > 4)
        {
            throw new ArgumentException("Enemy team cannot have more than 5 champions including the lane enemy.");
        }

        if (request.EnemyTeam.Distinct(StringComparer.OrdinalIgnoreCase).Count() != request.EnemyTeam.Count)
        {
            throw new ArgumentException("Enemy team cannot contain duplicate champions.");
        }

        if (request.EnemyTeam.Contains(request.LaneEnemy, StringComparer.OrdinalIgnoreCase))
        {
            throw new ArgumentException("Lane enemy cannot also be inside enemy team.");
        }

        var allChampions = await _championRepository.GetAllAsync();
        var championsByName = allChampions.ToDictionary(
            champion => champion.Name,
            StringComparer.OrdinalIgnoreCase);

        if (!championsByName.TryGetValue(request.LaneEnemy, out var laneEnemy))
        {
            throw new ArgumentException($"Unknown champion: {request.LaneEnemy}.");
        }

        var enemies = new List<string> { laneEnemy.Name };

        foreach (var enemyName in request.EnemyTeam)
        {
            if (!championsByName.TryGetValue(enemyName, out var enemy))
            {
                throw new ArgumentException($"Unknown champion: {enemyName}.");
            }

            enemies.Add(enemy.Name);
        }

        var rules = await _matchupRuleRepository.GetRulesForDraftAsync(
            role,
            enemies);

        var candidateChampions = allChampions
            .Where(champion => champion.Roles.Contains(role))
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
            Role = role,
            LaneEnemy = laneEnemy.Name,
            Recommendations = recommendations
        };
    }
}