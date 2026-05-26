using WildRiftCounterLab.Api.DTOs;

namespace WildRiftCounterLab.Api.Engine;

public class ReasonEngine
{
    public List<string> BuildReasons(string champion, DraftRequestDto request)
    {
        if (champion == "Malphite" && request.LaneEnemy == "Darius")
        {
            return new List<string>
            {
                "Armor scaling helps against physical damage",
                "Reliable engage",
                "Simple execution"
            };
        }

        if (champion == "Garen" && request.LaneEnemy == "Darius")
        {
            return new List<string>
            {
                "Safe lane option",
                "Good sustain",
                "Can survive short trades"
            };
        }

        if (champion == "Fiora")
        {
            return new List<string>
            {
                "Strong dueling",
                "Good side lane pressure",
                "Can punish tanks and fighters"
            };
        }

        return new List<string>
        {
            "Solid general pick"
        };
    }
}