namespace WildRiftCounterLab.Api.DTOs;

public class DraftRecommendationDto
{
    public string Champion { get; set; } = string.Empty;

    public int Score { get; set; }

    public string Plan { get; set; } = string.Empty;

    public List<string> Reasons { get; set; } = new();
}