namespace WildRiftCounterLab.Api.Repositories;

using Microsoft.EntityFrameworkCore;

using Data;
using Models;

public class MatchupRuleRepository
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
}