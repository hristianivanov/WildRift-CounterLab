namespace WildRiftCounterLab.Api.Data;

using Microsoft.EntityFrameworkCore;

using Models;


public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<MatchupRule> MatchupRules => Set<MatchupRule>();
    public DbSet<Champion> Champions => Set<Champion>();
}