using Microsoft.AspNetCore.Mvc;
using pointCounterBackend.DTOs.PointMatches;
using pointCounterBackend.Services.Interfaces;

namespace pointCounterBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PointMatchesController : ControllerBase
{
    private readonly IPointMatchService _pointMatchService;

    public PointMatchesController(IPointMatchService pointMatchService)
    {
        _pointMatchService = pointMatchService;
    }

    [HttpPost]
    public async Task<ActionResult<PointMatchDto>> Create(CreatePointMatchDto dto)
    {
        var match = await _pointMatchService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetByPublicId),
            new { publicId = match.PublicId },
            match);
    }

    [HttpGet("{publicId}")]
    public async Task<ActionResult<PointMatchDto>> GetByPublicId(string publicId)
    {
        var match = await _pointMatchService.GetByPublicIdAsync(publicId);

        if (match == null)
            return NotFound();

        return Ok(match);
    }

    [HttpPut("{publicId}/players/{playerId}/score")]
    public async Task<ActionResult<PointMatchDto>> UpdateScore(
        string publicId,
        int playerId,
        UpdatePointScoreDto dto)
    {
        var match = await _pointMatchService.UpdateScoreAsync(publicId, playerId, dto);

        if (match == null)
            return NotFound();

        return Ok(match);
    }

    [HttpPost("{publicId}/players")]
    public async Task<ActionResult<PointMatchDto>> AddPlayer(
        string publicId,
        AddPointMatchPlayerDto dto)
    {
        var match = await _pointMatchService.AddPlayerAsync(publicId, dto);

        if (match == null)
            return BadRequest("Matchen finns inte eller spelarna är låsta.");

        return Ok(match);
    }

    [HttpPut("{publicId}/players/{playerId}/name")]
    public async Task<ActionResult<PointMatchDto>> UpdatePlayerName(
        string publicId,
        int playerId,
        UpdatePointMatchPlayerNameDto dto)
    {
        var match = await _pointMatchService.UpdatePlayerNameAsync(publicId, playerId, dto);

        if (match == null)
            return BadRequest("Matchen finns inte, spelaren finns inte eller spelarna är låsta.");

        return Ok(match);
    }

    [HttpPost("{publicId}/restart")]
    public async Task<ActionResult<PointMatchDto>> Restart(string publicId)
    {
        var match = await _pointMatchService.RestartAsync(publicId);

        if (match == null)
            return NotFound();

        return Ok(match);
    }

    [HttpPost("{publicId}/clone")]
    public async Task<ActionResult<PointMatchDto>> Clone(string publicId)
    {
        var match = await _pointMatchService.CloneAsync(publicId);

        if (match == null)
            return NotFound();

        return Ok(match);
    }
}