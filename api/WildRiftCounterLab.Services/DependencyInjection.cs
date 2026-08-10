namespace WildRiftCounterLab.Services;

using Microsoft.Extensions.DependencyInjection;

using WildRiftCounterLab.Contracts;
using WildRiftCounterLab.Engine;
using WildRiftCounterLab.Services.Mapping;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        ServicesMappingConfig.Register();

        services.AddScoped<DraftService>();
        services.AddScoped<MatchupRuleAdminService>();
        services.AddScoped<ChampionAdminService>();
        services.AddScoped<IChampionSyncService, ChampionSyncService>();

        services.AddScoped<ScoreEngine>();
        services.AddScoped<ReasonEngine>();
        services.AddScoped<PlanEngine>();

        return services;
    }
}