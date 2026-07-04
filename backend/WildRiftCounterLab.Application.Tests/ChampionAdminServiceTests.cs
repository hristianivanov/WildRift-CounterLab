using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Interfaces;
using WildRiftCounterLab.Application.Services;
using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Tests;

public class ChampionAdminServiceTests
{
    [Fact]
    public async Task CreateAsync_RejectsDuplicateChampion()
    {
        var service = CreateService(champions: new List<Champion>
        {
            new() { Name = "Malphite" }
        });
        var request = ValidRequest();
        request.Name = "malphite";

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(request));

        Assert.Equal("A champion with this name already exists.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_RejectsInvalidRole()
    {
        var service = CreateService();
        var request = ValidRequest();
        request.Roles = new List<string> { "Roamer" };

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(request));

        Assert.Equal("Invalid role: Roamer.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_RejectsDuplicateRolesAndTags()
    {
        var service = CreateService();
        var duplicateRolesRequest = ValidRequest();
        duplicateRolesRequest.Roles = new List<string> { "Baron", "baron" };

        var roleException = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(duplicateRolesRequest));

        var duplicateTagsRequest = ValidRequest();
        duplicateTagsRequest.Tags = new List<string> { "tank", "TANK" };

        var tagException = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.CreateAsync(duplicateTagsRequest));

        Assert.Equal("Roles cannot contain duplicates.", roleException.Message);
        Assert.Equal("Tags cannot contain duplicates.", tagException.Message);
    }

    [Fact]
    public async Task DeleteAsync_RejectsReferencedChampion()
    {
        var champion = new Champion
        {
            Id = 1,
            Name = "Malphite",
            Roles = new List<string> { "Baron" },
            Tags = new List<string> { "tank" }
        };
        var service = CreateService(
            champions: new List<Champion> { champion },
            rules: new List<MatchupRule>
            {
                new() { Champion = "Malphite", EnemyChampion = "Darius" }
            });

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.DeleteAsync(champion.Id));

        Assert.Equal("Champion cannot be deleted while matchup rules reference it.", exception.Message);
    }

    [Fact]
    public async Task UpdateAsync_RejectsRenamingReferencedChampion()
    {
        var champion = new Champion
        {
            Id = 1,
            Name = "Malphite",
            Roles = new List<string> { "Baron" },
            Tags = new List<string> { "tank" }
        };
        var service = CreateService(
            champions: new List<Champion> { champion },
            rules: new List<MatchupRule>
            {
                new() { Champion = "Malphite", EnemyChampion = "Darius" }
            });

        var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
            service.UpdateAsync(champion.Id, new UpdateChampionRequestDto
            {
                Name = "New Malphite",
                Roles = new List<string> { "Baron" },
                Tags = new List<string> { "tank" }
            }));

        Assert.Equal("Champion name cannot be changed while matchup rules reference it.", exception.Message);
    }

    [Fact]
    public async Task CreateAsync_CreatesValidChampionWithTrimmedValues()
    {
        var repository = new FakeChampionRepository();
        var service = CreateService(championRepository: repository);
        var request = new CreateChampionRequestDto
        {
            Name = "  Ornn  ",
            Roles = new List<string> { " baron ", " support " },
            Tags = new List<string> { " tank ", " engage " }
        };

        var result = await service.CreateAsync(request);

        Assert.Equal(1, result.Id);
        Assert.Equal("Ornn", result.Name);
        Assert.Equal(new List<string> { "Baron", "Support" }, result.Roles);
        Assert.Equal(new List<string> { "tank", "engage" }, result.Tags);
        Assert.Single(repository.Champions);
    }

    private static CreateChampionRequestDto ValidRequest()
    {
        return new CreateChampionRequestDto
        {
            Name = "Ornn",
            Roles = new List<string> { "Baron" },
            Tags = new List<string> { "tank" }
        };
    }

    private static ChampionAdminService CreateService(
        List<Champion>? champions = null,
        List<MatchupRule>? rules = null,
        FakeChampionRepository? championRepository = null)
    {
        return new ChampionAdminService(
            championRepository ?? new FakeChampionRepository(champions),
            new FakeMatchupRuleRepository(rules));
    }

    private sealed class FakeChampionRepository : IChampionRepository
    {
        public FakeChampionRepository(List<Champion>? champions = null)
        {
            Champions = champions ?? new List<Champion>();
        }

        public List<Champion> Champions { get; }

        public Task<List<Champion>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Champions);
        }

        public Task<Champion?> GetByIdAsync(int id)
        {
            return Task.FromResult(Champions.SingleOrDefault(champion => champion.Id == id));
        }

        public Task<Champion?> GetByNameAsync(string name)
        {
            return Task.FromResult(Champions.SingleOrDefault(champion =>
                champion.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }

        public Task AddAsync(Champion champion)
        {
            champion.Id = Champions.Count == 0 ? 1 : Champions.Max(item => item.Id) + 1;
            Champions.Add(champion);
            return Task.CompletedTask;
        }

        public Task AddRangeAsync(IEnumerable<Champion> champions, CancellationToken cancellationToken = default)
        {
            Champions.AddRange(champions);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Champion champion)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Champion champion)
        {
            Champions.Remove(champion);
            return Task.CompletedTask;
        }

        public Task DeleteRangeAsync(IEnumerable<Champion> champions, CancellationToken cancellationToken = default)
        {
            foreach (var c in champions) Champions.Remove(c);
            return Task.CompletedTask;
        }

        public Task<bool> ExistsByNameAsync(string name)
        {
            return Task.FromResult(Champions.Any(champion =>
                champion.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }
    }

    private sealed class FakeMatchupRuleRepository : IMatchupRuleRepository
    {
        private readonly List<MatchupRule> _rules;

        public FakeMatchupRuleRepository(List<MatchupRule>? rules = null)
        {
            _rules = rules ?? new List<MatchupRule>();
        }

        public Task<List<MatchupRule>> GetRulesForDraftAsync(string role, List<string> enemies, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_rules);
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
}