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

        services.AddScoped<IExternalAiExplanationProvider>(_ =>
        {
            var provider = configuration["Ai:Provider"] ?? "Groq";

            return provider.Equals("Groq", StringComparison.OrdinalIgnoreCase)
                ? new GroqAiExplanationProvider(configuration)
                : provider.Equals("Gemini", StringComparison.OrdinalIgnoreCase)
                    ? new GeminiAiExplanationProvider(configuration)
                    : throw new InvalidOperationException(
                        $"Unsupported AI provider '{provider}'. Use 'Groq' or 'Gemini'.");
        });
        services.AddScoped<IAiExplanationProvider, CachedAiExplanationProvider>();

        services.AddScoped<IChampionRepository, ChampionRepository>();
        services.AddScoped<IMatchupRuleRepository, MatchupRuleRepository>();
        services.AddScoped<IAiExplanationCacheRepository, AiExplanationCacheRepository>();

        return services;
    }
}
