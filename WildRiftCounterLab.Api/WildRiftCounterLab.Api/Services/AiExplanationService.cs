namespace WildRiftCounterLab.Api.Services;

using Mscc.GenerativeAI;

using DTOs;

public class AiExplanationService
{
    private readonly IConfiguration _configuration;

    public AiExplanationService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<AiExplanationResponseDto> ExplainAsync(AiExplanationRequestDto request)
    {
        var apiKey = _configuration["Gemini:ApiKey"];
        var model = _configuration["Gemini:Model"] ?? "gemini-2.5-flash";

        var googleAI = new GoogleAI(apiKey: apiKey);

        var generativeModel = googleAI.GenerativeModel(model: model);

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
        };
    }
}