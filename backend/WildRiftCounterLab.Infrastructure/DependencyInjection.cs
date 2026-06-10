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
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured. Ensure appsettings.json or environment variables provide it.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

        services.AddScoped<IAiExplanationProvider, GeminiAiExplanationProvider>();

        services.AddScoped<IChampionRepository, ChampionRepository>();
        services.AddScoped<IMatchupRuleRepository, MatchupRuleRepository>();

        return services;
    }
}
