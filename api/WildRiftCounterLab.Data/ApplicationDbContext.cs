namespace WildRiftCounterLab.Data;

using Microsoft.EntityFrameworkCore;

using WildRiftCounterLab.Data.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<MatchupRule> MatchupRules => Set<MatchupRule>();
    public DbSet<Champion> Champions => Set<Champion>();
    public DbSet<AiExplanationCache> AiExplanationCaches => Set<AiExplanationCache>();
    public DbSet<MatchupTip> MatchupTips => Set<MatchupTip>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AiExplanationCache>()
            .HasIndex(cache => cache.CacheKey)
            .IsUnique();

        // Hot path: GetRulesForDraftAsync filters by Role + EnemyChampion on every recommendation request.
        modelBuilder.Entity<MatchupRule>()
            .HasIndex(rule => new { rule.Role, rule.EnemyChampion });

        // Hot path: GetTipsForDraftAsync filters by Champion + EnemyChampion.
        modelBuilder.Entity<MatchupTip>()
            .HasIndex(tip => new { tip.Champion, tip.EnemyChampion });
    }
}