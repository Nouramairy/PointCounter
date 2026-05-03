using Microsoft.AspNetCore.Mvc;
using pointCounterBackend.DTOs.Players;
using pointCounterBackend.Services.Interfaces;

namespace pointCounterBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayersController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpGet]
    public async Task<ActionResult<List<PlayerDto>>> GetAll()
    {
        var players = await _playerService.GetAllAsync();
        return Ok(players);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PlayerDto>> GetById(int id)
    {
        var player = await _playerService.GetByIdAsync(id);

        if (player == null)
            return NotFound();

        return Ok(player);
    }

    [HttpPost]
    public async Task<ActionResult<PlayerDto>> Create(CreatePlayerDto dto)
    {
        var player = await _playerService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = player.Id },
            player);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdatePlayerDto dto)
    {
        var result = await _playerService.UpdateAsync(id, dto);

        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _playerService.DeleteAsync(id);

        if (!result)
            return NotFound();

        return NoContent();
    }
}