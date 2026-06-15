namespace WildRiftCounterLab.Application;

using Engine;
using Mapping;

using Microsoft.Extensions.DependencyInjection;

using Services;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ApplicationMappingConfig.Register();

        services.AddScoped<DraftService>();
        services.AddScoped<MatchupRuleAdminService>();
        services.AddScoped<ChampionAdminService>();

        services.AddScoped<ScoreEngine>();
        services.AddScoped<ReasonEngine>();
        services.AddScoped<PlanEngine>();

        return services;
    }
}
