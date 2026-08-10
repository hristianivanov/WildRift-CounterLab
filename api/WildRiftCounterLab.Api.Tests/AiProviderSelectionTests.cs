namespace WildRiftCounterLab.Api.Tests;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using WildRiftCounterLab.Contracts;
using WildRiftCounterLab.Infrastructure;
using WildRiftCounterLab.Infrastructure.AI;

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
            services.GetRequiredKeyedService<IAiExplanationProvider>("external"));
    }

    [Fact]
    public void AddInfrastructure_SelectsGeminiByName()
    {
        using var services = BuildServices("Gemini");

        Assert.IsType<GeminiAiExplanationProvider>(
            services.GetRequiredKeyedService<IAiExplanationProvider>("external"));
    }

    [Fact]
    public void AddInfrastructure_RejectsUnsupportedProvider()
    {
        var exception = Assert.Throws<InvalidOperationException>(() => BuildServices("Unknown"));

        Assert.Contains("Unsupported AI provider", exception.Message);
    }

    private static ServiceProvider BuildServices(string? providerName)
    {
        var values = new Dictionary<string, string?>
        {
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