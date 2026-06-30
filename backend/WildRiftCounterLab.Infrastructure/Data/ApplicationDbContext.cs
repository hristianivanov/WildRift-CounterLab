using Microsoft.EntityFrameworkCore;

using WildRiftCounterLab.Domain.Entities;


namespace WildRiftCounterLab.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<MatchupRule> MatchupRules => Set<MatchupRule>();
    public DbSet<Champion> Champions => Set<Champion>();
    public DbSet<AiExplanationCache> AiExplanationCaches => Set<AiExplanationCache>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AiExplanationCache>()
            .HasIndex(cache => cache.CacheKey)
            .IsUnique();

        // Hot path: GetRulesForDraftAsync filters by Role + EnemyChampion on every recommendation request.
        modelBuilder.Entity<MatchupRule>()
            .HasIndex(rule => new { rule.Role, rule.EnemyChampion });
    }
}
