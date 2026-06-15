using System.Net;
using System.Net.Http.Json;

using Microsoft.AspNetCore.Hosting;

using WildRiftCounterLab.Application.DTOs;

namespace WildRiftCounterLab.Api.Tests;

public class ApiEndpointTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ApiEndpointTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Recommendations_ValidRequest_ReturnsOk()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/draft/recommendations",
            new DraftRecommendationRequestDto
            {
                Role = "Baron",
                LaneEnemy = "Darius"
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<DraftRecommendationResponseDto>();

        Assert.NotNull(body);
        Assert.NotEmpty(body.Recommendations);
    }

    [Fact]
    public async Task Recommendations_CommonDraftWithSennaAndJhin_ReturnsRankedRecommendations()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/draft/recommendations",
            new DraftRecommendationRequestDto
            {
                Role = "Baron",
                LaneEnemy = "Darius",
                EnemyTeam = new List<string> { "Senna", "Jhin", "Olaf" }
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadFromJsonAsync<DraftRecommendationResponseDto>();

        Assert.NotNull(body);
        Assert.NotEmpty(body.Recommendations);
        Assert.Contains(
            body.Recommendations.Take(3),
            recommendation => recommendation.Champion == "Malphite");
        Assert.DoesNotContain(body.Recommendations, recommendation =>
            recommendation.Champion is "Darius" or "Olaf");
    }

    [Fact]
    public async Task Champions_SeedIncludesSennaAndJhin()
    {
        var response = await _client.GetAsync("/api/champions");
        var champions = await response.Content.ReadFromJsonAsync<List<ChampionDto>>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(champions);
        Assert.Contains(champions, champion => champion.Name == "Senna");
        Assert.Contains(champions, champion => champion.Name == "Jhin");
    }

    [Fact]
    public async Task Health_ReturnsOkStatus()
    {
        var response = await _client.GetAsync("/api/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var health = await response.Content.ReadFromJsonAsync<HealthResponse>();

        Assert.NotNull(health);
        Assert.Equal("ok", health.Status);
    }

    [Fact]
    public async Task ApiDocumentation_ExplicitlyEnabledInProduction_ExposesScalarAndOpenApi()
    {
        using var client = _factory
            .WithWebHostBuilder(builder =>
                builder.UseSetting("ApiDocumentation:EnabledInProduction", "true"))
            .CreateClient();

        var scalarResponse = await client.GetAsync("/scalar");
        var openApiResponse = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, scalarResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, openApiResponse.StatusCode);
    }

    [Fact]
    public async Task AiExplain_UsesStandaloneSingleExplanationEndpoint()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/ai/explain",
            new AiExplanationRequestDto
            {
                Role = "Baron",
                LaneEnemy = "Darius",
                Champion = "Malphite",
                Score = 84,
                Reasons = new List<string> { "Armor scaling helps against physical damage" },
                Plan = "Play short trades early."
            });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var explanation = await response.Content.ReadFromJsonAsync<AiExplanationResponseDto>();

        Assert.NotNull(explanation);
        Assert.Equal("Explanation for Malphite", explanation.Explanation);
    }

    [Fact]
    public async Task Recommendations_InvalidRole_ReturnsStandardBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/draft/recommendations",
            new DraftRecommendationRequestDto
            {
                Role = "Roamer",
                LaneEnemy = "Darius"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();

        Assert.NotNull(error);
        Assert.Equal("Invalid role.", error.Error);
        Assert.False(string.IsNullOrWhiteSpace(error.TraceId));
    }

    [Fact]
    public async Task Recommendations_InvalidChampion_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/draft/recommendations",
            new DraftRecommendationRequestDto
            {
                Role = "Baron",
                LaneEnemy = "Unknown"
            });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();

        Assert.NotNull(error);
        Assert.Equal("Unknown champion: Unknown.", error.Error);
        Assert.False(string.IsNullOrWhiteSpace(error.TraceId));
    }

    [Fact]
    public async Task Recommendations_InvalidModelState_ReturnsStandardBadRequest()
    {
        using var content = new StringContent(
            """{"role":"Baron","laneEnemy":""",
            System.Text.Encoding.UTF8,
            "application/json");

        var response = await _client.PostAsync("/api/draft/recommendations", content);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();

        Assert.NotNull(error);
        Assert.Equal("Invalid request.", error.Error);
        Assert.False(string.IsNullOrWhiteSpace(error.Details));
        Assert.False(string.IsNullOrWhiteSpace(error.TraceId));
    }

    [Fact]
    public async Task Champions_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/champions");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var champions = await response.Content.ReadFromJsonAsync<List<ChampionDto>>();

        Assert.NotNull(champions);
        Assert.NotEmpty(champions);
    }

    [Fact]
    public async Task Champions_CreateValidChampion_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/champions",
            CreateChampionRequest("Api Test Champion"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var champion = await response.Content.ReadFromJsonAsync<ChampionDto>();

        Assert.NotNull(champion);
        Assert.True(champion.Id > 0);
        Assert.Equal("Api Test Champion", champion.Name);
    }

    [Fact]
    public async Task Champions_CreateDuplicateChampion_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/champions",
            CreateChampionRequest("malphite"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();

        Assert.NotNull(error);
        Assert.Equal("A champion with this name already exists.", error.Error);
    }

    [Fact]
    public async Task Champions_GetMissingChampion_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/champions/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();

        Assert.NotNull(error);
        Assert.Equal("Champion with id 999999 was not found.", error.Error);
    }

    [Fact]
    public async Task Champions_DeleteReferencedChampion_ReturnsBadRequest()
    {
        var championsResponse = await _client.GetAsync("/api/champions");
        var champions = await championsResponse.Content.ReadFromJsonAsync<List<ChampionDto>>();
        var malphite = Assert.Single(champions!, champion => champion.Name == "Malphite");

        var response = await _client.DeleteAsync($"/api/champions/{malphite.Id}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();

        Assert.NotNull(error);
        Assert.Equal("Champion cannot be deleted while matchup rules reference it.", error.Error);
    }

    [Fact]
    public async Task MatchupRules_CreateValidRule_ReturnsCreated()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/matchup-rules",
            CreateRuleRequest("Support", "Malphite", "Darius"));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var rule = await response.Content.ReadFromJsonAsync<MatchupRuleDto>();

        Assert.NotNull(rule);
        Assert.True(rule.Id > 0);
        Assert.Equal("Support", rule.Role);
    }

    [Fact]
    public async Task MatchupRules_CreateDuplicateRule_ReturnsBadRequest()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/matchup-rules",
            CreateRuleRequest("Baron", "Malphite", "Darius"));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();

        Assert.NotNull(error);
        Assert.Equal("A matchup rule already exists for this role and champion matchup.", error.Error);
    }

    [Fact]
    public async Task MatchupRules_GetAll_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/matchup-rules");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var rules = await response.Content.ReadFromJsonAsync<List<MatchupRuleDto>>();

        Assert.NotNull(rules);
        Assert.NotEmpty(rules);
    }

    [Fact]
    public async Task MatchupRules_GetMissingId_ReturnsNotFound()
    {
        var response = await _client.GetAsync("/api/matchup-rules/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();

        Assert.NotNull(error);
        Assert.Equal("Matchup rule with id 999999 was not found.", error.Error);
        Assert.False(string.IsNullOrWhiteSpace(error.TraceId));
    }

    [Fact]
    public async Task MatchupRules_DeleteExistingRule_ReturnsNoContent()
    {
        var createResponse = await _client.PostAsJsonAsync(
            "/api/matchup-rules",
            CreateRuleRequest("Jungle", "Malphite", "Darius"));
        var rule = await createResponse.Content.ReadFromJsonAsync<MatchupRuleDto>();

        Assert.NotNull(rule);

        var deleteResponse = await _client.DeleteAsync($"/api/matchup-rules/{rule.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);

        var getResponse = await _client.GetAsync($"/api/matchup-rules/{rule.Id}");

        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    private static CreateMatchupRuleRequestDto CreateRuleRequest(
        string role,
        string champion,
        string enemyChampion)
    {
        return new CreateMatchupRuleRequestDto
        {
            Role = role,
            Champion = champion,
            EnemyChampion = enemyChampion,
            ScoreModifier = 10,
            Reason = "Integration test rule",
            Plan = "Play around the matchup."
        };
    }

    private static CreateChampionRequestDto CreateChampionRequest(string name)
    {
        return new CreateChampionRequestDto
        {
            Name = name,
            Roles = new List<string> { "Baron" },
            Tags = new List<string> { "tank" }
        };
    }

    private sealed record HealthResponse(string Status);
}
