using WildRiftCounterLab.Application.DTOs;

namespace WildRiftCounterLab.Application.Interfaces;

public interface IAiExplanationService
{
    Task<AiExplanationResponseDto> ExplainAsync(AiExplanationRequestDto request);
}
