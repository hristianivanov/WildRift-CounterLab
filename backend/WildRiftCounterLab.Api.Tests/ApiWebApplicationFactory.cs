using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Interfaces;
using WildRiftCounterLab.Infrastructure.Data;

namespace WildRiftCounterLab.Api.Tests;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"WildRiftCounterLab-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Production");
        builder.UseSetting(
            "ConnectionStrings:DefaultConnection",
            "Host=localhost;Database=integration-tests;Username=integration-tests");
        builder.ConfigureLogging(logging => logging.ClearProviders());

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ApplicationDbContext>();
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<IAiExplanationProvider>();

            var providerConfigurations = services
                .Where(descriptor =>
                    descriptor.ServiceType.IsGenericType &&
                    descriptor.ServiceType.Name == "IDbContextOptionsConfiguration`1" &&
                    descriptor.ServiceType.GenericTypeArguments.Contains(typeof(ApplicationDbContext)))
                .ToList();

            foreach (var descriptor in providerConfigurations)
            {
                services.Remove(descriptor);
            }

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
            services.AddScoped<IAiExplanationProvider, FakeAiExplanationProvider>();
        });
    }

    private sealed class FakeAiExplanationProvider : IAiExplanationProvider
    {
        public Task<AiExplanationResponseDto> ExplainAsync(AiExplanationRequestDto request)
        {
            return Task.FromResult(new AiExplanationResponseDto
            {
                Explanation = $"Explanation for {request.Champion}"
            });
        }

        public Task<IReadOnlyDictionary<string, string>> ExplainBatchAsync(
            IReadOnlyCollection<AiExplanationRequestDto> requests,
            CancellationToken cancellationToken = default)
        {
            IReadOnlyDictionary<string, string> explanations = requests.ToDictionary(
                request => request.Champion,
                request => $"Explanation for {request.Champion}",
                StringComparer.OrdinalIgnoreCase);

            return Task.FromResult(explanations);
        }
    }
}
