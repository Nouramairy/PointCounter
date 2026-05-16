using Microsoft.AspNetCore.Mvc;
using pointCounterBackend.DTOs.Teams;
using pointCounterBackend.Services.Interfaces;

namespace pointCounterBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeamsController : ControllerBase
{
    private readonly ITeamService _teamService;

    public TeamsController(ITeamService teamService)
    {
        _teamService = teamService;
    }

    // GET: api/teams
    [HttpGet]
    public async Task<ActionResult<List<TeamDto>>> GetAll()
    {
        var teams = await _teamService.GetAllAsync();
        return Ok(teams);
    }

    // GET: api/teams/1
    [HttpGet("{id}")]
    public async Task<ActionResult<TeamDto>> GetById(int id)
    {
        var team = await _teamService.GetByIdAsync(id);

        if (team == null)
            return NotFound();

        return Ok(team);
    }

    // POST: api/teams
    [HttpPost]
    public async Task<ActionResult<TeamDto>> Create(CreateTeamDto dto)
    {
        var team = await _teamService.CreateAsync(dto);

        if (team == null)
            return BadRequest("Ogiltiga spelar-id:n eller max antal spelare har överskridits.");

        return CreatedAtAction(nameof(GetById), new { id = team.Id }, team);
    }

    // PUT: api/teams/1
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateTeamDto dto)
    {
        var result = await _teamService.UpdateAsync(id, dto);

        if (!result)
            return BadRequest("Lag kunde inte sparas.");

        return NoContent();
    }

    // DELETE: api/teams/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _teamService.DeleteAsync(id);

        if (!result)
            return NotFound();

        return NoContent();
    }
}