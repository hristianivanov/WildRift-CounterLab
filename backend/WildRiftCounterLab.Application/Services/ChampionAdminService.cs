using Mapster;

using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Exceptions;
using WildRiftCounterLab.Application.Interfaces;
using WildRiftCounterLab.Domain.Constants;
using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Services;

public class ChampionAdminService
{
    private readonly IChampionRepository _championRepository;
    private readonly IMatchupRuleRepository _matchupRuleRepository;

    public ChampionAdminService(
        IChampionRepository championRepository,
        IMatchupRuleRepository matchupRuleRepository)
    {
        _championRepository = championRepository;
        _matchupRuleRepository = matchupRuleRepository;
    }

    public async Task<List<ChampionDto>> GetAllAsync()
    {
        var champions = await _championRepository.GetAllAsync();

        return champions.Adapt<List<ChampionDto>>();
    }

    public async Task<ChampionDto> GetByIdAsync(int id)
    {
        return (await GetRequiredChampionAsync(id)).Adapt<ChampionDto>();
    }

    public async Task<ChampionDto> CreateAsync(CreateChampionRequestDto request)
    {
        var values = Validate(request.Name, request.Roles, request.Tags);

        if (await _championRepository.ExistsByNameAsync(values.Name))
        {
            throw new ArgumentException("A champion with this name already exists.");
        }

        var champion = request.Adapt<Champion>();
        champion.Name = values.Name;
        champion.Roles = values.Roles;
        champion.Tags = values.Tags;

        await _championRepository.AddAsync(champion);

        return champion.Adapt<ChampionDto>();
    }

    public async Task<ChampionDto> UpdateAsync(int id, UpdateChampionRequestDto request)
    {
        var champion = await GetRequiredChampionAsync(id);
        var values = Validate(request.Name, request.Roles, request.Tags);

        var nameChanged = !champion.Name.Equals(values.Name, StringComparison.OrdinalIgnoreCase);

        if (nameChanged && await _championRepository.ExistsByNameAsync(values.Name))
        {
            throw new ArgumentException("A champion with this name already exists.");
        }

        if (nameChanged && await _matchupRuleRepository.ExistsForChampionAsync(champion.Name))
        {
            throw new ArgumentException("Champion name cannot be changed while matchup rules reference it.");
        }

        champion.Name = values.Name;
        champion.Roles = values.Roles;
        champion.Tags = values.Tags;

        await _championRepository.UpdateAsync(champion);

        return champion.Adapt<ChampionDto>();
    }

    public async Task DeleteAsync(int id)
    {
        var champion = await GetRequiredChampionAsync(id);

        if (await _matchupRuleRepository.ExistsForChampionAsync(champion.Name))
        {
            throw new ArgumentException("Champion cannot be deleted while matchup rules reference it.");
        }

        await _championRepository.DeleteAsync(champion);
    }

    private async Task<Champion> GetRequiredChampionAsync(int id)
    {
        return await _championRepository.GetByIdAsync(id)
            ?? throw new NotFoundException($"Champion with id {id} was not found.");
    }

    private static (string Name, List<string> Roles, List<string> Tags) Validate(
        string? name,
        List<string>? roles,
        List<string>? tags)
    {
        var trimmedName = name?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(trimmedName))
        {
            throw new ArgumentException("Name is required.");
        }

        var trimmedRoles = (roles ?? new List<string>())
            .Select(role => role.Trim())
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .ToList();

        if (trimmedRoles.Count == 0)
        {
            throw new ArgumentException("At least one role is required.");
        }

        if (trimmedRoles.Distinct(StringComparer.OrdinalIgnoreCase).Count() != trimmedRoles.Count)
        {
            throw new ArgumentException("Roles cannot contain duplicates.");
        }

        var canonicalRoles = new List<string>();

        foreach (var role in trimmedRoles)
        {
            var canonicalRole = AllowedRoles.Values.SingleOrDefault(
                allowedRole => allowedRole.Equals(role, StringComparison.OrdinalIgnoreCase));

            if (canonicalRole is null)
            {
                throw new ArgumentException($"Invalid role: {role}.");
            }

            canonicalRoles.Add(canonicalRole);
        }

        var trimmedTags = (tags ?? new List<string>())
            .Select(tag => tag.Trim())
            .Where(tag => !string.IsNullOrWhiteSpace(tag))
            .ToList();

        if (trimmedTags.Count == 0)
        {
            throw new ArgumentException("At least one tag is required.");
        }

        if (trimmedTags.Distinct(StringComparer.OrdinalIgnoreCase).Count() != trimmedTags.Count)
        {
            throw new ArgumentException("Tags cannot contain duplicates.");
        }

        return (trimmedName, canonicalRoles, trimmedTags);
    }

}
