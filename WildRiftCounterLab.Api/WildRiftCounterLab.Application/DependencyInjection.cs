namespace WildRiftCounterLab.Application;

using Microsoft.Extensions.DependencyInjection;

using Engine;
using Services;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<DraftService>();

        services.AddScoped<ScoreEngine>();
        services.AddScoped<ReasonEngine>();
        services.AddScoped<PlanEngine>();

        return services;
    }
}