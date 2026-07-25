namespace WildRiftCounterLab.Domain.Constants;

public static class AllowedRoles
{
    public static readonly HashSet<string> Values = new()
    {
        "Baron",
        "Jungle",
        "Mid",
        "Dragon",
        "Support"
    };
}