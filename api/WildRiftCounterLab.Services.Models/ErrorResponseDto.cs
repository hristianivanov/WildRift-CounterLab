namespace WildRiftCounterLab.Services.Models;

public class ErrorResponseDto
{
    public string Error { get; set; } = string.Empty;

    public string? Details { get; set; }

    public string? TraceId { get; set; }
}
