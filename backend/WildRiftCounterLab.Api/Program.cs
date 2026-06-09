using System;
using System.Linq;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using WildRiftCounterLab.Application;
using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Exceptions;
using WildRiftCounterLab.Infrastructure;
using WildRiftCounterLab.Infrastructure.Data;
using WildRiftCounterLab.Infrastructure.Seed;

namespace WildRiftCounterLab.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers()
            .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
            {
                policy
                    .WithOrigins("http://localhost:5173", "https://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
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

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            DbSeeder.Seed(db);
        }

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

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsJsonAsync(new ErrorResponseDto
                {
                    Error = "Internal server error.",
                    TraceId = context.TraceIdentifier
                });
            });
        });

        app.UseHttpsRedirection();

        app.UseCors("Frontend");

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
