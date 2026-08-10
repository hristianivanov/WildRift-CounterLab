using System.ComponentModel.DataAnnotations;

namespace WildRiftCounterLab.Services.Models;

public class CreateChampionRequestDto
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public List<string> Roles { get; set; } = new();

    public List<string> Tags { get; set; } = new();
}
