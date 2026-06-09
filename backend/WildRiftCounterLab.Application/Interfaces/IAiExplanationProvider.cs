using WildRiftCounterLab.Application.DTOs;

namespace WildRiftCounterLab.Application.Interfaces;

public interface IAiExplanationProvider
{
    Task<AiExplanationResponseDto> ExplainAsync(AiExplanationRequestDto request);
}
