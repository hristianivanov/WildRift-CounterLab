using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Exceptions;
using WildRiftCounterLab.Application.Interfaces;
using WildRiftCounterLab.Domain.Constants;
using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Services;

public class MatchupRuleAdminService
{
    private readonly IMatchupRuleRepository _matchupRuleRepository;
    private readonly IChampionRepository _championRepository;

    public MatchupRuleAdminService(
        IMatchupRuleRepository matchupRuleRepository,
        IChampionRepository championRepository)
    {
        _matchupRuleRepository = matchupRuleRepository;
        _championRepository = championRepository;
    }

    public async Task<List<MatchupRuleDto>> GetAllAsync()
    {
        var rules = await _matchupRuleRepository.GetAllAsync();

        return rules.Select(Map).ToList();
    }

    public async Task<MatchupRuleDto> GetByIdAsync(int id)
    {
        var rule = await GetRequiredRuleAsync(id);

        return Map(rule);
    }

    public async Task<MatchupRuleDto> CreateAsync(CreateMatchupRuleRequestDto request)
    {
        var values = await ValidateAsync(
            request.Role,
            request.Champion,
            request.EnemyChampion,
            request.ScoreModifier,
            request.Reason);

        if (await _matchupRuleRepository.ExistsAsync(values.Role, values.Champion, values.EnemyChampion))
        {
            throw new ArgumentException("A matchup rule already exists for this role and champion matchup.");
        }

        var rule = new MatchupRule
        {
            Role = values.Role,
            Champion = values.Champion,
            EnemyChampion = values.EnemyChampion,
            ScoreModifier = request.ScoreModifier,
            Reason = request.Reason.Trim(),
            Plan = request.Plan?.Trim() ?? string.Empty
        };

        await _matchupRuleRepository.AddAsync(rule);

        return Map(rule);
    }

    public async Task<MatchupRuleDto> UpdateAsync(int id, UpdateMatchupRuleRequestDto request)
    {
        var rule = await GetRequiredRuleAsync(id);
        var values = await ValidateAsync(
            request.Role,
            request.Champion,
            request.EnemyChampion,
            request.ScoreModifier,
            request.Reason);

        var matchupChanged =
            !rule.Role.Equals(values.Role, StringComparison.OrdinalIgnoreCase) ||
            !rule.Champion.Equals(values.Champion, StringComparison.OrdinalIgnoreCase) ||
            !rule.EnemyChampion.Equals(values.EnemyChampion, StringComparison.OrdinalIgnoreCase);

        if (matchupChanged &&
            await _matchupRuleRepository.ExistsAsync(values.Role, values.Champion, values.EnemyChampion))
        {
            throw new ArgumentException("A matchup rule already exists for this role and champion matchup.");
        }

        rule.Role = values.Role;
        rule.Champion = values.Champion;
        rule.EnemyChampion = values.EnemyChampion;
        rule.ScoreModifier = request.ScoreModifier;
        rule.Reason = request.Reason.Trim();
        rule.Plan = request.Plan?.Trim() ?? string.Empty;

        await _matchupRuleRepository.UpdateAsync(rule);

        return Map(rule);
    }

    public async Task DeleteAsync(int id)
    {
        var rule = await GetRequiredRuleAsync(id);

        await _matchupRuleRepository.DeleteAsync(rule);
    }

    private async Task<MatchupRule> GetRequiredRuleAsync(int id)
    {
        return await _matchupRuleRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Matchup rule with id {id} was not found.");
    }

    private async Task<(string Role, string Champion, string EnemyChampion)> ValidateAsync(
        string role,
        string champion,
        string enemyChampion,
        int scoreModifier,
        string reason)
    {
        var canonicalRole = AllowedRoles.Values.SingleOrDefault(
            allowedRole => allowedRole.Equals(role, StringComparison.OrdinalIgnoreCase));

        if (canonicalRole is null)
        {
            throw new ArgumentException("Invalid role.");
        }

        if (scoreModifier is < -50 or > 50)
        {
            throw new ArgumentException("Score modifier must be between -50 and 50.");
        }

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason is required.");
        }

        var champions = await _championRepository.GetAllAsync();
        var championsByName = champions.ToDictionary(
            item => item.Name,
            StringComparer.OrdinalIgnoreCase);

        if (!championsByName.TryGetValue(champion, out var canonicalChampion))
        {
            throw new ArgumentException($"Unknown champion: {champion}.");
        }

        if (!championsByName.TryGetValue(enemyChampion, out var canonicalEnemyChampion))
        {
            throw new ArgumentException($"Unknown champion: {enemyChampion}.");
        }

        return (canonicalRole, canonicalChampion.Name, canonicalEnemyChampion.Name);
    }

    private static MatchupRuleDto Map(MatchupRule rule)
    {
        return new MatchupRuleDto
        {
            Id = rule.Id,
            Role = rule.Role,
            Champion = rule.Champion,
            EnemyChampion = rule.EnemyChampion,
            ScoreModifier = rule.ScoreModifier,
            Reason = rule.Reason,
            Plan = rule.Plan
        };
    }
}