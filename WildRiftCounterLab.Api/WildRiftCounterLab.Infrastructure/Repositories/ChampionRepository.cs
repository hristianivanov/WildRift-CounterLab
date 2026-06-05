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

    public async Task<List<Champion>> GetAllAsync()
    {
        return await _context.Champions.ToListAsync();
    }
}
