using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using WildRiftCounterLab.Application.DTOs;
using WildRiftCounterLab.Application.Services;

namespace WildRiftCounterLab.Api.Controllers;

[ApiController]
[Route("api/matchup-rules")]
public class MatchupRulesController : ControllerBase
{
    private readonly MatchupRuleAdminService _matchupRuleAdminService;

    public MatchupRulesController(MatchupRuleAdminService matchupRuleAdminService)
    {
        _matchupRuleAdminService = matchupRuleAdminService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<MatchupRuleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _matchupRuleAdminService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MatchupRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetById(int id)
    {
        return Ok(await _matchupRuleAdminService.GetByIdAsync(id));
    }

    [HttpPost]
    [ProducesResponseType(typeof(MatchupRuleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateMatchupRuleRequestDto request)
    {
        var rule = await _matchupRuleAdminService.CreateAsync(request);

        return CreatedAtAction(nameof(GetById), new { id = rule.Id }, rule);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(MatchupRuleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMatchupRuleRequestDto request)
    {
        return Ok(await _matchupRuleAdminService.UpdateAsync(id, request));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Delete(int id)
    {
        await _matchupRuleAdminService.DeleteAsync(id);

        return NoContent();
    }
}