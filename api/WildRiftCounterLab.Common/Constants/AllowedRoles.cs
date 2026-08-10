namespace WildRiftCounterLab.Common;

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
