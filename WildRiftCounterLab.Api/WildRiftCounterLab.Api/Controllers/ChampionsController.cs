namespace WildRiftCounterLab.Api.Controllers;

using Data;

using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/[controller]")]
public class ChampionsController : ControllerBase
{
    private readonly ChampionDataProvider _championDataProvider;

    public ChampionsController(ChampionDataProvider championDataProvider)
    {
        _championDataProvider = championDataProvider;
    }

    [HttpGet]
    public IActionResult GetChampions()
    {
        var champions = _championDataProvider.GetChampions();

        return Ok(champions);
    }
}