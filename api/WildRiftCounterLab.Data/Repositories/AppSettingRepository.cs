namespace WildRiftCounterLab.Data.Repositories;

using Microsoft.EntityFrameworkCore;

using WildRiftCounterLab.Contracts;
using WildRiftCounterLab.Data.Models;

public sealed class AppSettingRepository : IAppSettingRepository
{
    private readonly ApplicationDbContext _db;

    public AppSettingRepository(ApplicationDbContext db) => _db = db;

    public async Task<string?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        var setting = await _db.AppSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
        return setting?.Value;
    }

    public async Task SetAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        var setting = await _db.AppSettings.FirstOrDefaultAsync(s => s.Key == key, cancellationToken);
        if (setting is null)
        {
            _db.AppSettings.Add(new AppSetting { Key = key, Value = value, UpdatedAt = DateTime.UtcNow });
        }
        else
        {
            setting.Value = value;
            setting.UpdatedAt = DateTime.UtcNow;
        }
        await _db.SaveChangesAsync(cancellationToken);
    }
}
