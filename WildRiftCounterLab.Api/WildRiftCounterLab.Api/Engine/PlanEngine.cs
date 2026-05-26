using WildRiftCounterLab.Api.DTOs;

namespace WildRiftCounterLab.Api.Engine;

public class PlanEngine
{
    public string BuildPlan(string champion, DraftRequestDto request)
    {
        if (champion == "Malphite" && request.LaneEnemy == "Darius")
        {
            return "Play short trades early and wait for armor scaling.";
        }

        if (champion == "Garen" && request.LaneEnemy == "Darius")
        {
            return "Avoid extended bleed trades and sustain through passive.";
        }

        if (champion == "Fiora")
        {
            return "Pressure side lane and force isolated duels.";
        }

        if (champion == "Camille")
        {
            return "Farm safely early, then punish with mobility after first item.";
        }

        return "Play stable early game and scale into mid game fights.";
    }
}