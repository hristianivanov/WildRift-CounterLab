namespace WildRiftCounterLab.Infrastructure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using WildRiftCounterLab.Contracts;

using WildRiftCounterLab.Infrastructure.AI;
using WildRiftCounterLab.Infrastructure.ExternalApis.DataDragon;

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

        services.AddKeyedScoped<IAiExplanationProvider>("external", (_, _) =>
            aiProvider.Equals("Groq", StringComparison.OrdinalIgnoreCase)
                ? (IAiExplanationProvider)new GroqAiExplanationProvider(configuration)
                : new GeminiAiExplanationProvider(configuration));
        services.AddScoped<IAiExplanationProvider, CachedAiExplanationProvider>();

        services.AddHttpClient("DataDragon", client =>
        {
            client.Timeout = TimeSpan.FromSeconds(15);
        });
        services.AddScoped<IDataDragonClient, DataDragonClient>();

        return services;
    }
}