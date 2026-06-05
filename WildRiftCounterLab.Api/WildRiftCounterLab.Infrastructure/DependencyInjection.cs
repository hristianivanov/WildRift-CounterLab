namespace WildRiftCounterLab.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Application.Interfaces;
using AI;
using Data;
using Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IAiExplanationProvider, GeminiAiExplanationProvider>();

        services.AddScoped<IChampionRepository, ChampionRepository>();
        services.AddScoped<IMatchupRuleRepository, MatchupRuleRepository>();

        return services;
    }
}
