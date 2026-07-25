namespace WildRiftCounterLab.Api;

using System.Threading.RateLimiting;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

using Scalar.AspNetCore;

using Application;
using Application.DTOs;
using Application.Exceptions;

using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Seed;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers()
            .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; });

        var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
        builder.WebHost.UseUrls($"http://*:{port}");

        builder.Services.AddHealthChecks();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
            {
                var allowedOrigins = builder.Configuration
                    .GetSection("Frontend:AllowedOrigins")
                    .Get<string[]>();

                if (allowedOrigins is { Length: > 0 })
                {
                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
                // If no origins are configured, the policy allows nothing — CORS requests are denied.
            });
        });

        builder.Services.AddRateLimiter(options =>
        {
            options.AddFixedWindowLimiter("draft", limiter =>
            {
                limiter.PermitLimit = 30;
                limiter.Window = TimeSpan.FromMinutes(1);
                limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiter.QueueLimit = 5;
            });

            options.AddFixedWindowLimiter("ai", limiter =>
            {
                limiter.PermitLimit = 10;
                limiter.Window = TimeSpan.FromMinutes(1);
                limiter.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                limiter.QueueLimit = 2;
            });

            options.OnRejected = async (context, cancellationToken) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.HttpContext.Response.WriteAsJsonAsync(
                    new ErrorResponseDto
                    {
                        Error = "Too many requests. Please slow down.",
                        TraceId = context.HttpContext.TraceIdentifier
                    },
                    cancellationToken);
            };
        });

        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var details = string.Join(
                    " ",
                    context.ModelState.Values
                        .SelectMany(value => value.Errors)
                        .Select(error => error.ErrorMessage)
                        .Where(message => !string.IsNullOrWhiteSpace(message)));

                return new BadRequestObjectResult(new ErrorResponseDto
                {
                    Error = "Invalid request.",
                    Details = string.IsNullOrWhiteSpace(details) ? null : details,
                    TraceId = context.HttpContext.TraceIdentifier
                });
            };
        });

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);

        // Swashbuckle generates the OpenAPI document consumed by Scalar.
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        var startupLogger = app.Services.GetRequiredService<ILogger<Program>>();
        var configuredOrigins = builder.Configuration
            .GetSection("Frontend:AllowedOrigins")
            .Get<string[]>();

        if (configuredOrigins is not { Length: > 0 })
        {
            startupLogger.LogWarning(
                "Frontend:AllowedOrigins is not configured. All cross-origin requests will be denied.");
        }

        // Configure the HTTP request pipeline.
        var apiDocumentationEnabled =
            app.Environment.IsDevelopment() ||
            app.Configuration.GetValue<bool>("ApiDocumentation:EnabledInProduction");

        if (apiDocumentationEnabled)
        {
            app.UseSwagger();
            app.MapScalarApiReference("/scalar", options =>
                options.WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json"));
        }

        app.UseHttpsRedirection();


        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (app.Configuration.GetValue<bool>("Database:ApplyMigrationsOnStartup"))
            {
                db.Database.Migrate();
            }

            DbSeeder.Seed(db);
        }

        app.UseHealthChecks("/health");

        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var exceptionFeature =
                    context.Features.Get<IExceptionHandlerFeature>();

                var exception = exceptionFeature?.Error;

                context.Response.ContentType = "application/json";

                if (exception is ArgumentException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;

                    await context.Response.WriteAsJsonAsync(new ErrorResponseDto
                    {
                        Error = exception.Message,
                        TraceId = context.TraceIdentifier
                    });

                    return;
                }

                if (exception is NotFoundException)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;

                    await context.Response.WriteAsJsonAsync(new ErrorResponseDto
                    {
                        Error = exception.Message,
                        TraceId = context.TraceIdentifier
                    });

                    return;
                }

                var logger = context.RequestServices
                    .GetRequiredService<ILogger<Program>>();

                logger.LogError(exception, "Unhandled exception for {Method} {Path}",
                    context.Request.Method,
                    context.Request.Path);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsJsonAsync(new ErrorResponseDto
                {
                    Error = "Internal server error.",
                    TraceId = context.TraceIdentifier
                });
            });
        });

        app.UseCors("Frontend");

        app.UseRateLimiter();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
