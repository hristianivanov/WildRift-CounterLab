namespace WildRiftCounterLab.Application.DTOs;

public class ScoreBreakdownDto
{
    public int LaneScore { get; set; }

    public int TeamScore { get; set; }

    public int RoleFitScore { get; set; }

    public int SafetyScore { get; set; }

    public int ScalingScore { get; set; }

    public int UtilityScore { get; set; }

    public int TotalScore { get; set; }
}