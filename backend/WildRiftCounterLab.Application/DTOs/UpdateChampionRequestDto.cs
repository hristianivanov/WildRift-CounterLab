namespace WildRiftCounterLab.Application.DTOs;

public class UpdateChampionRequestDto
{
    public string Name { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = new();

    public List<string> Tags { get; set; } = new();
}