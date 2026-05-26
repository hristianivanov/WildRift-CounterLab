namespace WildRiftCounterLab.Api.Engine;

using DTOs;


public class ScoreEngine
{
    public int CalculateScore(string champion, DraftRequestDto request)
    {
        var score = 50;

        if (request.Role == "Baron")
        {
            score += 10;
        }

        if (request.LaneEnemy == "Darius" && champion == "Malphite")
        {
            score += 30;
        }

        if (request.LaneEnemy == "Darius" && champion == "Garen")
        {
            score += 20;
        }

        if (request.EnemyTeam.Contains("Senna") && champion == "Malphite")
        {
            score += 8;
        }

        if (request.EnemyTeam.Contains("Olaf") && champion == "Malphite")
        {
            score += 5;
        }

        return score;
    }
}