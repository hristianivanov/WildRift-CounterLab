namespace WildRiftCounterLab.Application.DTOs;

public class DraftRecommendationDto
{
    public string Champion { get; set; } = string.Empty;

    public int Score { get; set; }

    public List<string> Reasons { get; set; } = new();

    public string Plan { get; set; } = string.Empty;

    public string? AiExplanation { get; set; }
}