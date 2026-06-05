using WildRiftCounterLab.Domain.Entities;

namespace WildRiftCounterLab.Application.Interfaces;

public interface IChampionRepository
{
    Task<List<Champion>> GetAllAsync();
}