using System.Threading.Tasks;

using WildRiftCounterLab.Application.Interfaces;


namespace WildRiftCounterLab.Api.Controllers;

using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class ChampionsController : ControllerBase
{
    private readonly IChampionRepository _championRepository;

    public ChampionsController(IChampionRepository championRepository)
    {
        _championRepository = championRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetChampions()
    {
        var champions = await _championRepository.GetAllAsync();

        return Ok(champions);
    }
}