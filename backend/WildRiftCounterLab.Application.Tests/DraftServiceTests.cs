using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Engine;
using WildRiftCounterLab.Application.Interfaces;
using WildRiftCounterLab.Application.Services;
using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Tests;

public class DraftServiceTests
{
    [Fact]
    public async Task GetRecommendations_RejectsInvalidRole()
    {
        var service = CreateService();

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetRecommendations(new DraftRecommendationRequestDto
            {
                Role = "Roamer",
                LaneEnemy = "Darius"
            }));

        Assert.Equal("Invalid role.", exception.Message);
    }

    [Fact]
    public async Task GetRecommendations_RejectsUnknownLaneEnemy()
    {
        var service = CreateService();

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetRecommendations(new DraftRecommendationRequestDto
            {
                Role = "Baron",
                LaneEnemy = "Unknown"
            }));

        Assert.Equal("Unknown champion: Unknown.", exception.Message);
    }

    [Fact]
    public async Task GetRecommendations_RejectsDuplicateEnemiesCaseInsensitively()
    {
        var service = CreateService();

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.GetRecommendations(new DraftRecommendationRequestDto
            {
                Role = "Baron",
                LaneEnemy = "Darius",
                EnemyTeam = new List<string> { "Dr. Mundo", "dr. mundo" }
            }));

        Assert.Equal("Enemy team cannot contain duplicate champions.", exception.Message);
    }

    [Fact]
    public async Task GetRecommendations_RanksMalphiteAboveGarenAgainstDarius()
    {
        var service = CreateService();

        var response = await service.GetRecommendations(new DraftRecommendationRequestDto
        {
            Role = "Baron",
            LaneEnemy = "Darius"
        });

        var malphite = Assert.Single(response.Recommendations, recommendation =>
            recommendation.Champion == "Malphite");
        var garen = Assert.Single(response.Recommendations, recommendation =>
            recommendation.Champion == "Garen");

        Assert.True(malphite.Score > garen.Score);
    }

    [Fact]
    public async Task GetRecommendations_IncludesScoreBreakdown()
    {
        var service = CreateService();

        var response = await service.GetRecommendations(new DraftRecommendationRequestDto
        {
            Role = "Baron",
            LaneEnemy = "Darius"
        });

        var recommendation = Assert.Single(response.Recommendations, item =>
            item.Champion == "Malphite");

        Assert.Equal(recommendation.Score, recommendation.ScoreBreakdown.TotalScore);
        Assert.Equal(30, recommendation.ScoreBreakdown.LaneScore);
        Assert.True(recommendation.ScoreBreakdown.RoleFitScore > 0);
    }

    [Fact]
    public async Task GetRecommendations_DoesNotCallAiWhenExplanationIsDisabled()
    {
        var aiProvider = new FakeAiExplanationProvider();
        var service = CreateService(aiProvider);

        var response = await service.GetRecommendations(new DraftRecommendationRequestDto
        {
            Role = "Baron",
            LaneEnemy = "Darius",
            IncludeAiExplanation = false
        });

        Assert.Equal(0, aiProvider.CallCount);
        Assert.All(response.Recommendations, recommendation =>
            Assert.Null(recommendation.AiExplanation));
    }

    [Fact]
    public async Task GetRecommendations_AddsAiExplanationsToTopThreeRecommendations()
    {
        var aiProvider = new FakeAiExplanationProvider();
        var service = CreateService(aiProvider);

        var response = await service.GetRecommendations(new DraftRecommendationRequestDto
        {
            Role = "Baron",
            LaneEnemy = "Darius",
            IncludeAiExplanation = true
        });

        Assert.Equal(3, aiProvider.CallCount);
        Assert.All(response.Recommendations.Take(3), recommendation =>
            Assert.Equal($"Explanation for {recommendation.Champion}", recommendation.AiExplanation));
        Assert.All(response.Recommendations.Skip(3), recommendation =>
            Assert.Null(recommendation.AiExplanation));

        var firstRecommendation = response.Recommendations[0];
        var firstRequest = aiProvider.Requests[0];

        Assert.Equal("Baron", firstRequest.Role);
        Assert.Equal("Darius", firstRequest.LaneEnemy);
        Assert.Empty(firstRequest.EnemyTeam);
        Assert.Equal(firstRecommendation.Champion, firstRequest.Champion);
        Assert.Equal(firstRecommendation.Score, firstRequest.Score);
        Assert.Equal(firstRecommendation.Reasons, firstRequest.Reasons);
        Assert.Equal(firstRecommendation.Plan, firstRequest.Plan);
    }

    [Fact]
    public async Task GetRecommendations_ReturnsDeterministicRecommendationsWhenAiFails()
    {
        var aiProvider = new FakeAiExplanationProvider(shouldFail: true);
        var service = CreateService(aiProvider);

        var response = await service.GetRecommendations(new DraftRecommendationRequestDto
        {
            Role = "Baron",
            LaneEnemy = "Darius",
            IncludeAiExplanation = true
        });

        Assert.Equal(3, aiProvider.CallCount);
        Assert.All(response.Recommendations.Take(3), recommendation =>
            Assert.Equal("AI explanation unavailable.", recommendation.AiExplanation));
        Assert.Equal("Malphite", response.Recommendations[0].Champion);
        Assert.Equal(response.Recommendations[0].Score, response.Recommendations[0].ScoreBreakdown.TotalScore);
    }

    private static DraftService CreateService(FakeAiExplanationProvider? aiProvider = null)
    {
        var champions = new List<Champion>
        {
            new()
            {
                Name = "Malphite",
                Roles = new List<string> { "Baron", "Support" },
                Tags = new List<string> { "tank", "engage", "anti-ad" }
            },
            new()
            {
                Name = "Garen",
                Roles = new List<string> { "Baron" },
                Tags = new List<string> { "fighter", "safe", "sustain" }
            },
            new()
            {
                Name = "Darius",
                Roles = new List<string> { "Baron" },
                Tags = new List<string> { "fighter", "lane-bully", "ad" }
            },
            new()
            {
                Name = "Dr. Mundo",
                Roles = new List<string> { "Baron" },
                Tags = new List<string> { "tank", "sustain" }
            }
        };

        var rules = new List<MatchupRule>
        {
            new()
            {
                Role = "Baron",
                Champion = "Malphite",
                EnemyChampion = "Darius",
                ScoreModifier = 30,
                Reason = "Armor scaling helps against physical damage",
                Plan = "Play short trades early."
            },
            new()
            {
                Role = "Baron",
                Champion = "Garen",
                EnemyChampion = "Darius",
                ScoreModifier = 20,
                Reason = "Safe lane option with sustain",
                Plan = "Keep trades short."
            }
        };

        return new DraftService(
            new ScoreEngine(),
            new ReasonEngine(),
            new PlanEngine(),
            new FakeChampionRepository(champions),
            new FakeMatchupRuleRepository(rules),
            aiProvider ?? new FakeAiExplanationProvider());
    }

    private sealed class FakeChampionRepository : IChampionRepository
    {
        private readonly List<Champion> _champions;

        public FakeChampionRepository(List<Champion> champions)
        {
            _champions = champions;
        }

        public Task<List<Champion>> GetAllAsync()
        {
            return Task.FromResult(_champions);
        }

        public Task<Champion?> GetByIdAsync(int id)
        {
            return Task.FromResult(_champions.SingleOrDefault(champion => champion.Id == id));
        }

        public Task<Champion?> GetByNameAsync(string name)
        {
            return Task.FromResult(_champions.SingleOrDefault(champion =>
                champion.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }

        public Task AddAsync(Champion champion)
        {
            _champions.Add(champion);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Champion champion)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Champion champion)
        {
            _champions.Remove(champion);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsByNameAsync(string name)
        {
            return Task.FromResult(_champions.Any(champion =>
                champion.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }
    }

    private sealed class FakeMatchupRuleRepository : IMatchupRuleRepository
    {
        private readonly List<MatchupRule> _rules;

        public FakeMatchupRuleRepository(List<MatchupRule> rules)
        {
            _rules = rules;
        }

        public Task<List<MatchupRule>> GetRulesForDraftAsync(string role, List<string> enemies)
        {
            var rules = _rules
                .Where(rule =>
                    rule.Role.Equals(role, StringComparison.OrdinalIgnoreCase) &&
                    enemies.Contains(rule.EnemyChampion, StringComparer.OrdinalIgnoreCase))
                .ToList();

            return Task.FromResult(rules);
        }

        public Task<List<MatchupRule>> GetAllAsync()
        {
            return Task.FromResult(_rules);
        }

        public Task<MatchupRule?> GetByIdAsync(int id)
        {
            return Task.FromResult(_rules.SingleOrDefault(rule => rule.Id == id));
        }

        public Task AddAsync(MatchupRule rule)
        {
            _rules.Add(rule);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(MatchupRule rule)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(MatchupRule rule)
        {
            _rules.Remove(rule);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string role, string champion, string enemyChampion)
        {
            return Task.FromResult(_rules.Any(rule =>
                rule.Role.Equals(role, StringComparison.OrdinalIgnoreCase) &&
                rule.Champion.Equals(champion, StringComparison.OrdinalIgnoreCase) &&
                rule.EnemyChampion.Equals(enemyChampion, StringComparison.OrdinalIgnoreCase)));
        }

        public Task<bool> ExistsForChampionAsync(string championName)
        {
            return Task.FromResult(_rules.Any(rule =>
                rule.Champion.Equals(championName, StringComparison.OrdinalIgnoreCase) ||
                rule.EnemyChampion.Equals(championName, StringComparison.OrdinalIgnoreCase)));
        }
    }

    private sealed class FakeAiExplanationProvider : IAiExplanationProvider
    {
        private readonly bool _shouldFail;

        public FakeAiExplanationProvider(bool shouldFail = false)
        {
            _shouldFail = shouldFail;
        }

        public int CallCount { get; private set; }

        public List<AiExplanationRequestDto> Requests { get; } = new();

        public Task<AiExplanationResponseDto> ExplainAsync(AiExplanationRequestDto request)
        {
            CallCount++;
            Requests.Add(request);

            if (_shouldFail)
            {
                throw new InvalidOperationException("AI unavailable.");
            }

            return Task.FromResult(new AiExplanationResponseDto
            {
                Explanation = $"Explanation for {request.Champion}"
            });
        }
    }
}