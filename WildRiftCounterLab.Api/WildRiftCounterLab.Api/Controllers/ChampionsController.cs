namespace WildRiftCounterLab.Api.Controllers;

using Microsoft.AspNetCore.Mvc;

using Repositories;

[ApiController]
[Route("api/[controller]")]
public class ChampionsController : ControllerBase
{
    private readonly ChampionRepository _championRepository;

    public ChampionsController(ChampionRepository championRepository)
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