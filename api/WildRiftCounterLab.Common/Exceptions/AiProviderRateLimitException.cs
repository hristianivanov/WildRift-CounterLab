namespace WildRiftCounterLab.Common;

public class AiProviderRateLimitException : Exception
{
    public AiProviderRateLimitException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}