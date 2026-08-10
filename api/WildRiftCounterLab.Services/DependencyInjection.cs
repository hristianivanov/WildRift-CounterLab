using Microsoft.Extensions.DependencyInjection;

using WildRiftCounterLab.Services.Engine;
using WildRiftCounterLab.Services.Mapping;

namespace WildRiftCounterLab.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        ServicesMappingConfig.Register();

        services.AddScoped<DraftService>();
        services.AddScoped<MatchupRuleAdminService>();
        services.AddScoped<ChampionAdminService>();

        services.AddScoped<ScoreEngine>();
        services.AddScoped<ReasonEngine>();
        services.AddScoped<PlanEngine>();

        return services;
    }
}
