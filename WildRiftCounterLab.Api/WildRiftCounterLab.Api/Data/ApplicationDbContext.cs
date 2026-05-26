using Microsoft.EntityFrameworkCore;
using WildRiftCounterLab.Api.Models;

namespace WildRiftCounterLab.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Champion> Champions => Set<Champion>();
}