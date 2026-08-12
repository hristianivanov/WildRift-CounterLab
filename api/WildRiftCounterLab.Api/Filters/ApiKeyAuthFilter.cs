namespace WildRiftCounterLab.Api.Filters;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

using WildRiftCounterLab.Services.Models;

public sealed class ApiKeyAuthFilter : IAuthorizationFilter
{
    private const string ApiKeyHeader = "X-Api-Key";

    private readonly string _apiKey;

    public ApiKeyAuthFilter(IConfiguration configuration)
    {
        _apiKey = configuration["Admin:ApiKey"]
            ?? throw new InvalidOperationException("Admin:ApiKey is not configured.");
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if (!context.HttpContext.Request.Headers.TryGetValue(ApiKeyHeader, out var providedKey)
            || !string.Equals(providedKey, _apiKey, StringComparison.Ordinal))
        {
            context.Result = new ObjectResult(new ErrorResponseDto
            {
                Error = "Unauthorized.",
                TraceId = context.HttpContext.TraceIdentifier
            })
            {
                StatusCode = StatusCodes.Status401Unauthorized
            };
        }
    }
}
