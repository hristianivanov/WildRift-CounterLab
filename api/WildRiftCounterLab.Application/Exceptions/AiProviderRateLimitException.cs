namespace WildRiftCounterLab.Application.Exceptions;

public class AiProviderRateLimitException : Exception
{
    public AiProviderRateLimitException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
