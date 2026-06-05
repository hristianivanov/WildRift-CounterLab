namespace WildRiftCounterLab.Api;

using Application.Engine;
using Application.Interfaces;
using Application.Services;

using Infrastructure.AI;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Seed;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using WildRiftCounterLab.Application;
using WildRiftCounterLab.Infrastructure;

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

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}