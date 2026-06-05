using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using WildRiftCounterLab.Application.Engine;
using WildRiftCounterLab.Application.Services;
using WildRiftCounterLab.Application.Interfaces;
using WildRiftCounterLab.Infrastructure.AI;
using WildRiftCounterLab.Infrastructure.Data;
using WildRiftCounterLab.Infrastructure.Repositories;
using WildRiftCounterLab.Infrastructure.Seed;


namespace WildRiftCounterLab.Api;


using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers()
            .AddJsonOptions(options => { options.JsonSerializerOptions.PropertyNameCaseInsensitive = true; });

        builder.Services.AddScoped<DraftService>();
        builder.Services.AddScoped<AiExplanationService>();

        builder.Services.AddScoped<ScoreEngine>();
        builder.Services.AddScoped<ReasonEngine>();
        builder.Services.AddScoped<PlanEngine>();

        builder.Services.AddScoped<IChampionRepository, ChampionRepository>();
        builder.Services.AddScoped<IMatchupRuleRepository, MatchupRuleRepository>();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(
                builder.Configuration.GetConnectionString("DefaultConnection")));

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