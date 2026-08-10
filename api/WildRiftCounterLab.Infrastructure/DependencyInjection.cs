namespace WildRiftCounterLab.Infrastructure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using WildRiftCounterLab.Services.Interfaces;

using AI;
using Services;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var aiProvider = configuration["Ai:Provider"] ?? "Groq";

        if (!aiProvider.Equals("Groq", StringComparison.OrdinalIgnoreCase) &&
            !aiProvider.Equals("Gemini", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                $"Unsupported AI provider '{aiProvider}'. Use 'Groq' or 'Gemini'.");
        }

        services.AddScoped<IExternalAiExplanationProvider>(_ =>
            aiProvider.Equals("Groq", StringComparison.OrdinalIgnoreCase)
                ? new GroqAiExplanationProvider(configuration)
                : new GeminiAiExplanationProvider(configuration));
        services.AddScoped<IAiExplanationProvider, CachedAiExplanationProvider>();

        services.AddHttpClient("DataDragon", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(15);
        });
        services.AddScoped<IChampionSyncService, ChampionSyncService>();

        return services;
    }
}
