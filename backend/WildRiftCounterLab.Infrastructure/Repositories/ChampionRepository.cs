using Microsoft.EntityFrameworkCore;

using WildRiftCounterLab.Application.Interfaces;
using WildRiftCounterLab.Domain.Entities;
using WildRiftCounterLab.Infrastructure.Data;


namespace WildRiftCounterLab.Infrastructure.Repositories;

public class ChampionRepository : IChampionRepository
{
    private readonly ApplicationDbContext _context;

    public ChampionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Champion>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Champions
            .OrderBy(champion => champion.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Champion?> GetByIdAsync(int id)
    {
        return await _context.Champions.FindAsync(id);
    }

    public async Task<Champion?> GetByNameAsync(string name)
    {
        return await _context.Champions.FirstOrDefaultAsync(champion =>
            champion.Name.ToLower() == name.ToLower());
    }

    public async Task AddAsync(Champion champion)
    {
        _context.Champions.Add(champion);
        await _context.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<Champion> champions, CancellationToken cancellationToken = default)
    {
        _context.Champions.AddRange(champions);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Champion champion)
    {
        _context.Champions.Update(champion);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Champion champion)
    {
        _context.Champions.Remove(champion);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Champions.AnyAsync(champion =>
            champion.Name.ToLower() == name.ToLower());
    }
}