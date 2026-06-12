using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Engine;
using WildRiftCounterLab.Application.Interfaces;
using WildRiftCounterLab.Domain.Constants;
using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Services;

public class DraftService
{
    private const int AiExplanationLimit = 3;

    private readonly ScoreEngine _scoreEngine;
    private readonly ReasonEngine _reasonEngine;
    private readonly PlanEngine _planEngine;

    private readonly IChampionRepository _championRepository;
    private readonly IMatchupRuleRepository _matchupRuleRepository;
    private readonly IAiExplanationProvider _aiExplanationProvider;

    public DraftService(
        ScoreEngine scoreEngine,
        ReasonEngine reasonEngine,
        PlanEngine planEngine,
        IChampionRepository championRepository,
        IMatchupRuleRepository matchupRuleRepository,
        IAiExplanationProvider aiExplanationProvider)
    {
        _scoreEngine = scoreEngine;
        _reasonEngine = reasonEngine;
        _championRepository = championRepository;
        _matchupRuleRepository = matchupRuleRepository;
        _aiExplanationProvider = aiExplanationProvider;
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
        var enemyChampions = new List<Champion> { laneEnemy };

        foreach (var enemyName in request.EnemyTeam)
        {
            if (!championsByName.TryGetValue(enemyName, out var enemy))
            {
                throw new ArgumentException($"Unknown champion: {enemyName}.");
            }

            enemies.Add(enemy.Name);
            enemyChampions.Add(enemy);
        }

        var rules = await _matchupRuleRepository.GetRulesForDraftAsync(
            role,
            enemies);

        var candidateChampions = allChampions
            .Where(champion => champion.Roles.Contains(role))
            .ToList();

        var recommendations = candidateChampions
            .Select(champion =>
            {
                var scoreBreakdown = _scoreEngine.CalculateScore(
                    champion,
                    role,
                    laneEnemy,
                    enemyChampions,
                    rules);

                return new DraftRecommendationDto
                {
                    Champion = champion.Name,
                    Score = scoreBreakdown.TotalScore,
                    ScoreBreakdown = scoreBreakdown,
                    Reasons = _reasonEngine.BuildReasons(champion, enemyChampions, rules),
                    Plan = _planEngine.BuildPlan(champion, rules, enemyChampions)
                };
            })
            .OrderByDescending(x => x.Score)
            .Take(5)
            .ToList();

        if (request.IncludeAiExplanation)
        {
            var recommendationsToExplain = recommendations
                .Take(AiExplanationLimit)
                .ToList();

            try
            {
                var explanationRequests = recommendationsToExplain
                    .Select(recommendation =>
                        new AiExplanationRequestDto
                        {
                            Role = role,
                            LaneEnemy = laneEnemy.Name,
                            EnemyTeam = enemyChampions
                                .Skip(1)
                                .Select(enemy => enemy.Name)
                                .ToList(),
                            Champion = recommendation.Champion,
                            Score = recommendation.Score,
                            Reasons = recommendation.Reasons,
                            Plan = recommendation.Plan
                        })
                    .ToList();

                var explanations = await _aiExplanationProvider.ExplainBatchAsync(explanationRequests);

                foreach (var recommendation in recommendationsToExplain)
                {
                    recommendation.AiExplanation = explanations.TryGetValue(
                        recommendation.Champion,
                        out var explanation)
                        && !string.IsNullOrWhiteSpace(explanation)
                            ? explanation
                            : "AI explanation unavailable.";
                }
            }
            catch (Exception)
            {
                foreach (var recommendation in recommendationsToExplain)
                {
                    recommendation.AiExplanation = "AI explanation unavailable.";
                }
            }
        }

        return new DraftRecommendationResponseDto
        {
            Role = role,
            LaneEnemy = laneEnemy.Name,
            Recommendations = recommendations
        };
    }
}
