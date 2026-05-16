using Microsoft.AspNetCore.Mvc;
using pointCounterBackend.DTOs.Scoreboards;
using pointCounterBackend.Services.Interfaces;

namespace pointCounterBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScoreboardsController : ControllerBase
{
    private readonly IScoreboardService _scoreboardService;

    public ScoreboardsController(IScoreboardService scoreboardService)
    {
        _scoreboardService = scoreboardService;
    }

    [HttpGet("game/{gameId}")]
    public async Task<ActionResult<List<ScoreboardDto>>> GetByGameId(int gameId)
    {
        var scoreboard = await _scoreboardService.GetByGameIdAsync(gameId);
        return Ok(scoreboard);
    }

    [HttpPut]
    public async Task<ActionResult<ScoreboardDto>> UpdateScore(UpdateScoreboardDto dto)
    {
        var scoreboard = await _scoreboardService.UpdateScoreAsync(dto);

        if (scoreboard == null)
            return BadRequest("Matchen finns inte, eller laget ingår inte i matchen.");

        return Ok(scoreboard);
    }
}