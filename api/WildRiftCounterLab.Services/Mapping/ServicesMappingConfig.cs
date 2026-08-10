using Mapster;

using WildRiftCounterLab.Data.Models;
using WildRiftCounterLab.Services.Models;

namespace WildRiftCounterLab.Services.Mapping;

public static class ServicesMappingConfig
{
    public static void Register()
    {
        TypeAdapterConfig<Champion, ChampionDto>.NewConfig();
        TypeAdapterConfig<CreateChampionRequestDto, Champion>.NewConfig();

        TypeAdapterConfig<MatchupRule, MatchupRuleDto>.NewConfig();
        TypeAdapterConfig<CreateMatchupRuleRequestDto, MatchupRule>.NewConfig();
    }
}
