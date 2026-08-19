namespace WildRiftCounterLab.Api.Controllers;

using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Filters;
using Services;
using Services.Models;

[ApiController]
[Route("api/matchup-tips")]
public class MatchupTipsController : ControllerBase
{
    private readonly MatchupTipAdminService _matchupTipAdminService;

    public MatchupTipsController(MatchupTipAdminService matchupTipAdminService)
    {
        _matchupTipAdminService = matchupTipAdminService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<MatchupTipDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _matchupTipAdminService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MatchupTipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        return Ok(await _matchupTipAdminService.GetByIdAsync(id));
    }

    [HttpPost]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    [ProducesResponseType(typeof(MatchupTipDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateMatchupTipRequestDto request)
    {
        var tip = await _matchupTipAdminService.CreateAsync(request);

        return CreatedAtAction(nameof(GetById), new { id = tip.Id }, tip);
    }

    [HttpPut("{id:int}")]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    [ProducesResponseType(typeof(MatchupTipDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMatchupTipRequestDto request)
    {
        return Ok(await _matchupTipAdminService.UpdateAsync(id, request));
    }

    [HttpDelete("{id:int}")]
    [ServiceFilter(typeof(ApiKeyAuthFilter))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        await _matchupTipAdminService.DeleteAsync(id);

        return NoContent();
    }
}
