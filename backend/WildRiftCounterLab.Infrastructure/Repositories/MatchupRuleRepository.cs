using Microsoft.EntityFrameworkCore;

using WildRiftCounterLab.Application.Interfaces;
using WildRiftCounterLab.Domain.Entities;
using WildRiftCounterLab.Infrastructure.Data;


namespace WildRiftCounterLab.Infrastructure.Repositories;

public class MatchupRuleRepository : IMatchupRuleRepository
{
    private readonly ApplicationDbContext _context;

    public MatchupRuleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<MatchupRule>> GetRulesForDraftAsync(string role, List<string> enemies)
    {
        return await _context.MatchupRules
            .Where(rule =>
                rule.Role == role &&
                enemies.Contains(rule.EnemyChampion))
            .ToListAsync();
    }

    public async Task<List<MatchupRule>> GetAllAsync()
    {
        return await _context.MatchupRules
            .OrderBy(rule => rule.Role)
            .ThenBy(rule => rule.Champion)
            .ThenBy(rule => rule.EnemyChampion)
            .ToListAsync();
    }

    public async Task<MatchupRule?> GetByIdAsync(int id)
    {
        return await _context.MatchupRules.FindAsync(id);
    }

    public async Task AddAsync(MatchupRule rule)
    {
        _context.MatchupRules.Add(rule);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(MatchupRule rule)
    {
        _context.MatchupRules.Update(rule);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(MatchupRule rule)
    {
        _context.MatchupRules.Remove(rule);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string role, string champion, string enemyChampion)
    {
        return await _context.MatchupRules.AnyAsync(rule =>
            rule.Role == role &&
            rule.Champion == champion &&
            rule.EnemyChampion == enemyChampion);
    }

    public async Task<bool> ExistsForChampionAsync(string championName)
    {
        return await _context.MatchupRules.AnyAsync(rule =>
            rule.Champion == championName ||
            rule.EnemyChampion == championName);
    }
}