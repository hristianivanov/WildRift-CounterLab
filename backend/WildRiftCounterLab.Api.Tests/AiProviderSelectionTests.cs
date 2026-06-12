using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using WildRiftCounterLab.Infrastructure;
using WildRiftCounterLab.Infrastructure.AI;

namespace WildRiftCounterLab.Api.Tests;

public class AiProviderSelectionTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("Groq")]
    [InlineData("groq")]
    public void AddInfrastructure_SelectsGroqByDefaultAndByName(string? providerName)
    {
        using var services = BuildServices(providerName);

        Assert.IsType<GroqAiExplanationProvider>(
            services.GetRequiredService<IExternalAiExplanationProvider>());
    }

    [Fact]
    public void AddInfrastructure_SelectsGeminiByName()
    {
        using var services = BuildServices("Gemini");

        Assert.IsType<GeminiAiExplanationProvider>(
            services.GetRequiredService<IExternalAiExplanationProvider>());
    }

    [Fact]
    public void AddInfrastructure_RejectsUnsupportedProvider()
    {
        using var services = BuildServices("Unknown");

        var exception = Assert.Throws<InvalidOperationException>(() =>
            services.GetRequiredService<IExternalAiExplanationProvider>());

        Assert.Contains("Unsupported AI provider", exception.Message);
    }

    private static ServiceProvider BuildServices(string? providerName)
    {
        var values = new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] =
                "Host=localhost;Database=provider-tests;Username=provider-tests",
            ["Ai:Provider"] = providerName
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
        var services = new ServiceCollection();

        services.AddInfrastructure(configuration);

        return services.BuildServiceProvider();
    }
}
