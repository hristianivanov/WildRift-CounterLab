using System;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using WildRiftCounterLab.Application;
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
                    context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();

                var exception = exceptionFeature?.Error;

                context.Response.ContentType = "application/json";

                if (exception is ArgumentException)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;

                    await context.Response.WriteAsJsonAsync(new
                    {
                        error = exception.Message
                    });

                    return;
                }

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Internal server error."
                });
            });
        });

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}