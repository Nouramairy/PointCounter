using Microsoft.AspNetCore.Mvc;
using pointCounterBackend.DTOs.Games;
using pointCounterBackend.Services.Interfaces;

namespace pointCounterBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;

    public GamesController(IGameService gameService)
    {
        _gameService = gameService;
    }

    [HttpGet]
    public async Task<ActionResult<List<GameDto>>> GetAll()
    {
        var games = await _gameService.GetAllAsync();
        return Ok(games);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GameDto>> GetById(int id)
    {
        var game = await _gameService.GetByIdAsync(id);

        if (game == null)
            return NotFound();

        return Ok(game);
    }

    [HttpPost]
    public async Task<ActionResult<GameDto>> Create(CreateGameDto dto)
    {
        var game = await _gameService.CreateAsync(dto);

        if (game == null)
            return BadRequest("Invalid team IDs. A game requires at least one existing team.");

        return CreatedAtAction(nameof(GetById), new { id = game.Id }, game);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateGameDto dto)
    {
        var result = await _gameService.UpdateAsync(id, dto);

        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _gameService.DeleteAsync(id);

        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{gameId}/teams/{teamId}")]
    public async Task<IActionResult> AddTeamToGame(int gameId, int teamId)
    {
        var result = await _gameService.AddTeamToGameAsync(gameId, teamId);

        if (!result)
            return BadRequest("Game or team does not exist, or team is already added to this game.");

        return NoContent();
    }

    [HttpDelete("{gameId}/teams/{teamId}")]
    public async Task<IActionResult> RemoveTeamFromGame(int gameId, int teamId)
    {
        var result = await _gameService.RemoveTeamFromGameAsync(gameId, teamId);

        if (!result)
            return NotFound();

        return NoContent();
    }
}