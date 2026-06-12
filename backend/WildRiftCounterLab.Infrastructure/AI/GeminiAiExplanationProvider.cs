using System.Text.Json;

using Microsoft.Extensions.Configuration;

using Mscc.GenerativeAI;

using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Interfaces;

namespace WildRiftCounterLab.Infrastructure.AI;

public class GeminiAiExplanationProvider : IAiExplanationProvider
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly IConfiguration _configuration;

    public GeminiAiExplanationProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<AiExplanationResponseDto> ExplainAsync(AiExplanationRequestDto request)
    {
        var generativeModel = CreateGenerativeModel();

        var prompt = $"""
                      You are a Wild Rift draft assistant.

                      Explain this recommendation in a practical way.

                      Role: {request.Role}
                      Lane enemy: {request.LaneEnemy}
                      Enemy team: {string.Join(", ", request.EnemyTeam)}

                      Recommended champion: {request.Champion}
                      Score: {request.Score}

                      Engine reasons:
                      {string.Join("\n", request.Reasons.Select(reason => "- " + reason))}

                      Engine plan:
                      {request.Plan}

                      Return only this format:

                      Why strong:
                      - max 2 bullets

                      Lane plan:
                      - max 2 bullets

                      Main risk:
                      - max 1 bullet

                      Use simple language.
                      Keep the full answer under 120 words.
                      Do not use markdown headings.
                      Do not write long explanations.
                      Do not invent patch data.
                      """;

        var response = await generativeModel.GenerateContent(prompt);

        return new AiExplanationResponseDto
        {
            Explanation = response.Text
                ?? throw new InvalidOperationException("Gemini returned an empty explanation.")
        };
    }

    public async Task<IReadOnlyDictionary<string, string>> ExplainBatchAsync(
        IReadOnlyCollection<AiExplanationRequestDto> requests,
        CancellationToken cancellationToken = default)
    {
        if (requests.Count == 0)
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        var generativeModel = CreateGenerativeModel();
        var prompt = BuildBatchPrompt(requests);
        var response = await generativeModel.GenerateContent(
            prompt,
            cancellationToken: cancellationToken);

        return ParseBatchExplanations(response.Text);
    }

    private GenerativeModel CreateGenerativeModel()
    {
        var apiKey = _configuration["Gemini:ApiKey"];
        var model = _configuration["Gemini:Model"] ?? "gemini-2.5-flash";

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new InvalidOperationException("Gemini API key is not configured.");
        }

        var googleAI = new GoogleAI(apiKey: apiKey);

        return googleAI.GenerativeModel(model: model);
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
                You are a Wild Rift draft assistant.

                Explain the engine's top recommendations without changing or questioning their scores or ranking.

                Role: {{firstRequest.Role}}
                Lane enemy: {{firstRequest.LaneEnemy}}
                Enemy team: {{string.Join(", ", firstRequest.EnemyTeam)}}

                Top recommendations:
                {{recommendationContext}}

                Return strict JSON only in this shape:
                {
                  "explanations": [
                    {
                      "champion": "Malphite",
                      "explanation": "..."
                    }
                  ]
                }

                Return one explanation for every supplied champion.
                Each explanation must be under 90 words and practical.
                Include why the pick is strong, the lane plan, and the main risk.
                Do not use markdown, markdown tables, or code fences.
                Do not make patch claims or invent live statistics.
                """;
    }

    private static IReadOnlyDictionary<string, string> ParseBatchExplanations(string? responseText)
    {
        if (string.IsNullOrWhiteSpace(responseText))
        {
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        try
        {
            var json = ExtractJson(responseText);
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

    private static string ExtractJson(string responseText)
    {
        var trimmed = responseText.Trim();
        var firstBrace = trimmed.IndexOf('{');
        var lastBrace = trimmed.LastIndexOf('}');

        return firstBrace >= 0 && lastBrace > firstBrace
            ? trimmed[firstBrace..(lastBrace + 1)]
            : trimmed;
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
