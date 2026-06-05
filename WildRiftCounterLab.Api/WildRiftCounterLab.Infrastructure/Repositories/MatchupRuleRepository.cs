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
}