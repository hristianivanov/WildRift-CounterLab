using WildRiftCounterLab.Application.DTOs;

namespace WildRiftCounterLab.Application.Interfaces;

public interface IAiExplanationProvider
{
    Task<AiExplanationResponseDto> ExplainAsync(AiExplanationRequestDto request, CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, string>> ExplainBatchAsync(
        IReadOnlyCollection<AiExplanationRequestDto> requests,
        CancellationToken cancellationToken = default);
}
