namespace WildRiftCounterLab.Application.DTOs;

public class DraftRecommendationResponseDto
{
    public string Role { get; set; } = string.Empty;

    public string LaneEnemy { get; set; } = string.Empty;

    public List<DraftRecommendationDto> Recommendations { get; set; } = new();
}