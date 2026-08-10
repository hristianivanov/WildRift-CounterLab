using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using WildRiftCounterLab.Data.Repositories;
using WildRiftCounterLab.Contracts;

namespace WildRiftCounterLab.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddData(
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

        services.AddScoped<IChampionRepository, ChampionRepository>();
        services.AddScoped<IMatchupRuleRepository, MatchupRuleRepository>();
        services.AddScoped<IAiExplanationCacheRepository, AiExplanationCacheRepository>();

        return services;
    }
}
