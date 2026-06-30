using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

using Microsoft.Extensions.Configuration;

using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Exceptions;

namespace WildRiftCounterLab.Infrastructure.AI;

public class GroqAiExplanationProvider : IExternalAiExplanationProvider
{
    private static readonly HttpClient HttpClient = new()
    {
        BaseAddress = new Uri("https://api.groq.com/openai/v1/"),
        Timeout = TimeSpan.FromSeconds(40)
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IConfiguration _configuration;

    public GroqAiExplanationProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<AiExplanationResponseDto> ExplainAsync(AiExplanationRequestDto request, CancellationToken cancellationToken = default)
    {
        var explanation = await CompleteAsync(BuildSinglePrompt(request), cancellationToken);

        return new AiExplanationResponseDto { Explanation = explanation };
    }

    public async Task<IReadOnlyDictionary<string, string>> ExplainBatchAsync(
        IReadOnlyCollection<AiExplanationRequestDto> requests,
        CancellationToken cancellationToken = default)
    {
        if (requests.Count == 0)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        var responseText = await CompleteAsync(BuildBatchPrompt(requests), cancellationToken);

        return ParseBatchExplanations(responseText);
    }

    private async Task<string> CompleteAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        var apiKey = _configuration["Groq:ApiKey"];
        var model = _configuration["Groq:Model"] ?? "llama-3.1-8b-instant";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Groq API key is not configured.");
        }

        using var request = new HttpRequestMessage(HttpMethod.Post, "chat/completions");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
        request.Content = JsonContent.Create(new
        {
            model,
            temperature = 0.2,
            max_tokens = 500,
            messages = new[]
            {
                new
                {
                    role = "user",
                    content = prompt
                }
            }
        });

        using var response = await HttpClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            throw new AiProviderRateLimitException(
                "Groq provider rate limit reached.",
                new HttpRequestException("Groq returned HTTP 429."));
        }

        if (!response.IsSuccessStatusCode)
        {
            var details = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException(
                $"Groq request failed with HTTP {(int)response.StatusCode}: {details}");
        }

        var result = await response.Content.ReadFromJsonAsync<GroqChatResponse>(
            JsonOptions,
            cancellationToken);

        return result?.Choices.FirstOrDefault()?.Message.Content
            ?? throw new InvalidOperationException("Groq returned an empty explanation.");
    }

    private static string BuildSinglePrompt(AiExplanationRequestDto request)
    {
        return $"""
                You are a Wild Rift draft assistant. Explain the engine recommendation without changing its score.

                Role: {request.Role}
                Lane enemy: {request.LaneEnemy}
                Enemy team: {string.Join(", ", request.EnemyTeam)}
                Recommended champion: {request.Champion}
                Score: {request.Score}
                Engine reasons: {string.Join("; ", request.Reasons)}
                Engine plan: {request.Plan}

                Use this short structure:
                Why strong:
                Lane plan:
                Main risk:

                Keep the full response under 90 words.
                Do not use markdown tables, claim live patch data, or invent statistics.
                """;
    }

    private static string BuildBatchPrompt(IReadOnlyCollection<AiExplanationRequestDto> requests)
    {
        var firstRequest = requests.First();
        var recommendationContext = JsonSerializer.Serialize(
            requests.Select(request => new
            {
                request.Champion,
                request.Score,
                request.Reasons,
                request.Plan
            }),
            JsonOptions);

        return $$"""
                You are a Wild Rift draft assistant. Explain these engine recommendations without changing their scores.

                Role: {{firstRequest.Role}}
                Lane enemy: {{firstRequest.LaneEnemy}}
                Enemy team: {{string.Join(", ", firstRequest.EnemyTeam)}}
                Recommendations: {{recommendationContext}}

                Return strict JSON only:
                {
                  "explanations": [
                    {
                      "champion": "Champion name",
                      "explanation": "Why strong, lane plan, and main risk in under 90 words."
                    }
                  ]
                }

                Do not use markdown tables, claim live patch data, or invent statistics.
                """;
    }

    private static IReadOnlyDictionary<string, string> ParseBatchExplanations(string responseText)
    {
        try
        {
            var firstBrace = responseText.IndexOf('{');
            var lastBrace = responseText.LastIndexOf('}');
            var json = firstBrace >= 0 && lastBrace > firstBrace
                ? responseText[firstBrace..(lastBrace + 1)]
                : responseText;
            var response = JsonSerializer.Deserialize<BatchExplanationResponse>(json, JsonOptions);

            return response?.Explanations
                .Where(item =>
                    !string.IsNullOrWhiteSpace(item.Champion)
                    && !string.IsNullOrWhiteSpace(item.Explanation))
                .GroupBy(item => item.Champion, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(
                    group => group.Key,
                    group => group.First().Explanation,
                    StringComparer.OrdinalIgnoreCase)
                ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
        catch (JsonException)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
    }

    private sealed class GroqChatResponse
    {
        public List<GroqChoice> Choices { get; set; } = new();
    }

    private sealed class GroqChoice
    {
        public GroqMessage Message { get; set; } = new();
    }

    private sealed class GroqMessage
    {
        public string Content { get; set; } = string.Empty;
    }

    private sealed class BatchExplanationResponse
    {
        public List<BatchExplanationItem> Explanations { get; set; } = new();
    }

    private sealed class BatchExplanationItem
    {
        public string Champion { get; set; } = string.Empty;

        public string Explanation { get; set; } = string.Empty;
    }
}
