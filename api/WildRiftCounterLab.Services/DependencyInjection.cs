namespace WildRiftCounterLab.Services;

using Microsoft.Extensions.DependencyInjection;

using WildRiftCounterLab.Contracts;
using WildRiftCounterLab.Engine;
using WildRiftCounterLab.Services.Mapping;
using WildRiftCounterLab.Services.Patch;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        ServicesMappingConfig.Register();

        services.AddScoped<DraftService>();
        services.AddScoped<MatchupRuleAdminService>();
        services.AddScoped<ChampionAdminService>();
        services.AddScoped<IChampionSyncService, ChampionSyncService>();
        services.AddScoped<PatchCheckService>();
        services.AddSingleton<PatchMonitorService>();
        services.AddHostedService(sp => sp.GetRequiredService<PatchMonitorService>());

        services.AddScoped<ScoreEngine>();
        services.AddScoped<ReasonEngine>();
        services.AddScoped<PlanEngine>();

        return services;
    }
}