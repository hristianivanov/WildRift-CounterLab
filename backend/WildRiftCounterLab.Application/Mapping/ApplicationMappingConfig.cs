using Mapster;

using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Mapping;

public static class ApplicationMappingConfig
{
    public static void Register()
    {
        TypeAdapterConfig<Champion, ChampionDto>.NewConfig();
        TypeAdapterConfig<CreateChampionRequestDto, Champion>.NewConfig();

        TypeAdapterConfig<MatchupRule, MatchupRuleDto>.NewConfig();
        TypeAdapterConfig<CreateMatchupRuleRequestDto, MatchupRule>.NewConfig();
    }
}
