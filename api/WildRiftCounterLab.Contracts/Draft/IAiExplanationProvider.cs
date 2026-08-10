using WildRiftCounterLab.Services.Models;

namespace WildRiftCounterLab.Contracts;

public interface IAiExplanationProvider
{
    Task<AiExplanationResponseDto> ExplainAsync(AiExplanationRequestDto request, CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, string>> ExplainBatchAsync(
        IReadOnlyCollection<AiExplanationRequestDto> requests,
        CancellationToken cancellationToken = default);
}
