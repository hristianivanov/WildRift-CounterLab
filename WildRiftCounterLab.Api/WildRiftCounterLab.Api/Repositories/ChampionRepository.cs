namespace WildRiftCounterLab.Api.Repositories;

using Microsoft.EntityFrameworkCore;

using Data;
using Models;

public class ChampionRepository
{
    private readonly ApplicationDbContext _context;

    public ChampionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Champion>> GetAllAsync()
    {
        return await _context.Champions.ToListAsync();
    }
}
