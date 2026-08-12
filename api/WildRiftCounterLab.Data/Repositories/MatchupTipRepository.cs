namespace WildRiftCounterLab.Data.Repositories;

using Microsoft.EntityFrameworkCore;

using WildRiftCounterLab.Contracts;
using WildRiftCounterLab.Data.Models;

public class MatchupTipRepository : IMatchupTipRepository
{
    private readonly ApplicationDbContext _context;

    public MatchupTipRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MatchupTip>> GetTipsForDraftAsync(string champion, List<string> enemies, CancellationToken cancellationToken = default)
    {
        return await _context.MatchupTips
            .Where(tip =>
                tip.Champion == champion &&
                enemies.Contains(tip.EnemyChampion))
            .ToListAsync(cancellationToken);
    }

    public async Task<List<MatchupTip>> GetAllAsync()
    {
        return await _context.MatchupTips
            .OrderBy(tip => tip.Champion)
            .ThenBy(tip => tip.EnemyChampion)
            .ToListAsync();
    }

    public async Task<MatchupTip?> GetByIdAsync(int id)
    {
        return await _context.MatchupTips.FindAsync(id);
    }

    public async Task AddAsync(MatchupTip tip)
    {
        _context.MatchupTips.Add(tip);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MatchupTip tip)
    {
        _context.MatchupTips.Update(tip);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(MatchupTip tip)
    {
        _context.MatchupTips.Remove(tip);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string champion, string enemyChampion, string tip)
    {
        return await _context.MatchupTips.AnyAsync(t =>
            t.Champion == champion &&
            t.EnemyChampion == enemyChampion &&
            t.Tip == tip);
    }
}
