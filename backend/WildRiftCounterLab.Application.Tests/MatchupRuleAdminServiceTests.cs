using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Interfaces;
using WildRiftCounterLab.Application.Services;
using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Tests;

public class MatchupRuleAdminServiceTests
{
    [Fact]
    public async Task CreateAsync_RejectsInvalidRole()
    {
        var service = CreateService();
        var request = ValidRequest();
        request.Role = "Roamer";

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(request));

        Assert.Equal("Invalid role.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_RejectsUnknownChampion()
    {
        var service = CreateService();
        var request = ValidRequest();
        request.Champion = "Unknown";

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(request));

        Assert.Equal("Unknown champion: Unknown.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_RejectsDuplicateRule()
    {
        var existingRule = new MatchupRule
        {
            Role = "Baron",
            Champion = "Malphite",
            EnemyChampion = "Darius",
            Reason = "Existing"
        };
        var service = CreateService(new List<MatchupRule> { existingRule });

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(ValidRequest()));

        Assert.Equal("A matchup rule already exists for this role and champion matchup.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_RejectsModifierOutsideRange()
    {
        var service = CreateService();
        var request = ValidRequest();
        request.ScoreModifier = 51;

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(request));

        Assert.Equal("Score modifier must be between -50 and 50.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_CreatesValidRule()
    {
        var repository = new FakeMatchupRuleRepository();
        var service = CreateService(repository: repository);

        var result = await service.CreateAsync(ValidRequest());

        Assert.Equal(1, result.Id);
        Assert.Equal("Baron", result.Role);
        Assert.Equal("Malphite", result.Champion);
        Assert.Equal("Darius", result.EnemyChampion);
        Assert.Single(repository.Rules);
    }

    private static CreateMatchupRuleRequestDto ValidRequest()
    {
        return new CreateMatchupRuleRequestDto
        {
            Role = "Baron",
            Champion = "Malphite",
            EnemyChampion = "Darius",
            ScoreModifier = 20,
            Reason = "Reliable lane matchup",
            Plan = "Keep trades short."
        };
    }

    private static MatchupRuleAdminService CreateService(
        List<MatchupRule>? rules = null,
        FakeMatchupRuleRepository? repository = null)
    {
        repository ??= new FakeMatchupRuleRepository(rules);

        return new MatchupRuleAdminService(
            repository,
            new FakeChampionRepository(new List<Champion>
            {
                new() { Name = "Malphite" },
                new() { Name = "Darius" }
            }));
    }

    private sealed class FakeChampionRepository : IChampionRepository
    {
        private readonly List<Champion> _champions;

        public FakeChampionRepository(List<Champion> champions)
        {
            _champions = champions;
        }

        public Task<List<Champion>> GetAllAsync(CancellationToken cancellationToken = default)
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

        public Task AddRangeAsync(IEnumerable<Champion> champions, CancellationToken cancellationToken = default)
        {
            _champions.AddRange(champions);
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
        public FakeMatchupRuleRepository(List<MatchupRule>? rules = null)
        {
            Rules = rules ?? new List<MatchupRule>();
        }

        public List<MatchupRule> Rules { get; }

        public Task<List<MatchupRule>> GetRulesForDraftAsync(string role, List<string> enemies, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Rules);
        }

        public Task<List<MatchupRule>> GetAllAsync()
        {
            return Task.FromResult(Rules);
        }

        public Task<MatchupRule?> GetByIdAsync(int id)
        {
            return Task.FromResult(Rules.SingleOrDefault(rule => rule.Id == id));
        }

        public Task AddAsync(MatchupRule rule)
        {
            rule.Id = Rules.Count == 0 ? 1 : Rules.Max(item => item.Id) + 1;
            Rules.Add(rule);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(MatchupRule rule)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(MatchupRule rule)
        {
            Rules.Remove(rule);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsAsync(string role, string champion, string enemyChampion)
        {
            return Task.FromResult(Rules.Any(rule =>
                rule.Role.Equals(role, StringComparison.OrdinalIgnoreCase) &&
                rule.Champion.Equals(champion, StringComparison.OrdinalIgnoreCase) &&
                rule.EnemyChampion.Equals(enemyChampion, StringComparison.OrdinalIgnoreCase)));
        }

        public Task<bool> ExistsForChampionAsync(string championName)
        {
            return Task.FromResult(Rules.Any(rule =>
                rule.Champion.Equals(championName, StringComparison.OrdinalIgnoreCase) ||
                rule.EnemyChampion.Equals(championName, StringComparison.OrdinalIgnoreCase)));
        }
    }
}