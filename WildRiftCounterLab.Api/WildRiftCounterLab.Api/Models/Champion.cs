namespace WildRiftCounterLab.Api.Models;

public class Champion
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = new();

    public List<string> Tags { get; set; } = new();
}