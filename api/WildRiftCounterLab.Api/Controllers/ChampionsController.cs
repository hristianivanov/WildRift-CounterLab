namespace WildRiftCounterLab.Api.Controllers;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Contracts;
using Services;
using Services.Models;
using Services.Patch;

[ApiController]
[Route("api/[controller]")]
public class ChampionsController : ControllerBase
{
    private readonly ChampionAdminService _championAdminService;
    private readonly IChampionSyncService _championSyncService;
    private readonly PatchMonitorService _patchMonitor;

    public ChampionsController(
        ChampionAdminService championAdminService,
        IChampionSyncService championSyncService,
        PatchMonitorService patchMonitor)
    {
        _championAdminService = championAdminService;
        _championSyncService = championSyncService;
        _patchMonitor = patchMonitor;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ChampionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetChampions()
    {
        return Ok(await _championAdminService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ChampionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetChampion(int id)
    {
        return Ok(await _championAdminService.GetByIdAsync(id));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ChampionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateChampion([FromBody] CreateChampionRequestDto request)
    {
        var champion = await _championAdminService.CreateAsync(request);

        return CreatedAtAction(nameof(GetChampion), new { id = champion.Id }, champion);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ChampionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateChampion(int id, [FromBody] UpdateChampionRequestDto request)
    {
        return Ok(await _championAdminService.UpdateAsync(id, request));
    }

    [HttpPost("sync")]
    [ProducesResponseType(typeof(ChampionSyncResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SyncChampions(CancellationToken cancellationToken)
    {
        return Ok(await _championSyncService.SyncFromDataDragonAsync(cancellationToken));
    }

    [HttpPost("patch-check")]
    [ProducesResponseType(typeof(PatchCheckResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CheckPatch(CancellationToken cancellationToken)
    {
        return Ok(await _patchMonitor.CheckAndSyncAsync(cancellationToken));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteChampion(int id)
    {
        await _championAdminService.DeleteAsync(id);

        return NoContent();
    }
}