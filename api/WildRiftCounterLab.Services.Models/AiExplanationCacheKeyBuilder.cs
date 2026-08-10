namespace WildRiftCounterLab.Services.Models;

using System.Security.Cryptography;
using System.Text;

public static class AiExplanationCacheKeyBuilder
{
    public static string Build(AiExplanationRequestDto request)
    {
        var normalizedEnemyTeam = request.EnemyTeam
            .Select(Normalize)
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToList();
        var enemyTeamHash = Hash(string.Join("|", normalizedEnemyTeam));
        var keySource = string.Join(
            "|",
            Normalize(request.Role),
            Normalize(request.LaneEnemy),
            enemyTeamHash,
            Normalize(request.Champion));

        return Hash(keySource);
    }

    public static string BuildEnemyTeamHash(IReadOnlyCollection<string> enemyTeam)
    {
        return Hash(string.Join(
            "|",
            enemyTeam
                .Select(Normalize)
                .OrderBy(name => name, StringComparer.Ordinal)));
    }

    private static string Normalize(string value) => value.Trim().ToUpperInvariant();

    private static string Hash(string value)
        => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}