using Microsoft.EntityFrameworkCore;
using pointCounterBackend.Data;
using pointCounterBackend.DTOs.PointMatches;
using pointCounterBackend.Entities;
using pointCounterBackend.Services.Interfaces;

namespace pointCounterBackend.Services;

public class PointMatchService : IPointMatchService
{
    private readonly AppDbContext _context;

    public PointMatchService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PointMatchDto> CreateAsync(CreatePointMatchDto dto)
    {
        var match = new PointMatch
        {
            PublicId = Guid.NewGuid().ToString(),
            Name = dto.Name,
            HigherScoreWins = dto.HigherScoreWins,
            StartingScore = dto.StartingScore,
            PlayersLocked = dto.PlayersLocked && dto.PlayerNames.Any(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Players = dto.PlayerNames.Select(name => new PointMatchPlayer
            {
                Name = name,
                Score = dto.StartingScore,
                OriginalScore = dto.StartingScore
            }).ToList()
        };

        _context.PointMatches.Add(match);
        await _context.SaveChangesAsync();

        return MapToDto(match);
    }

    public async Task<PointMatchDto?> GetByPublicIdAsync(string publicId)
    {
        var match = await GetMatchAsync(publicId);

        if (match == null)
            return null;

        return MapToDto(match);
    }

    public async Task<PointMatchDto?> UpdateScoreAsync(
        string publicId,
        int playerId,
        UpdatePointScoreDto dto)
    {
        var match = await GetMatchAsync(publicId);

        if (match == null)
            return null;

        var player = match.Players.FirstOrDefault(p => p.Id == playerId);

        if (player == null)
            return null;

        player.Score = dto.Score;
        match.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDto(match);
    }

    public async Task<PointMatchDto?> AddPlayerAsync(
        string publicId,
        AddPointMatchPlayerDto dto)
    {
        var match = await GetMatchAsync(publicId);

        if (match == null)
            return null;

        if (match.PlayersLocked)
            return null;

        var player = new PointMatchPlayer
        {
            PointMatchId = match.Id,
            Name = dto.Name,
            Score = match.StartingScore,
            OriginalScore = match.StartingScore
        };

        match.Players.Add(player);
        match.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDto(match);
    }

    public async Task<PointMatchDto?> UpdatePlayerNameAsync(
        string publicId,
        int playerId,
        UpdatePointMatchPlayerNameDto dto)
    {
        var match = await GetMatchAsync(publicId);

        if (match == null)
            return null;

        if (match.PlayersLocked)
            return null;

        var player = match.Players.FirstOrDefault(p => p.Id == playerId);

        if (player == null)
            return null;

        player.Name = dto.Name;
        match.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDto(match);
    }

    public async Task<PointMatchDto?> RestartAsync(string publicId)
    {
        var match = await GetMatchAsync(publicId);

        if (match == null)
            return null;

        foreach (var player in match.Players)
        {
            player.Score = player.OriginalScore;
        }

        match.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return MapToDto(match);
    }

    public async Task<PointMatchDto?> CloneAsync(string publicId)
    {
        var oldMatch = await GetMatchAsync(publicId);

        if (oldMatch == null)
            return null;

        var newMatch = new PointMatch
        {
            PublicId = Guid.NewGuid().ToString(),
            Name = oldMatch.Name,
            HigherScoreWins = oldMatch.HigherScoreWins,
            StartingScore = oldMatch.StartingScore,
            PlayersLocked = oldMatch.PlayersLocked,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Players = oldMatch.Players.Select(player => new PointMatchPlayer
            {
                Name = player.Name,
                Score = oldMatch.StartingScore,
                OriginalScore = oldMatch.StartingScore
            }).ToList()
        };

        _context.PointMatches.Add(newMatch);
        await _context.SaveChangesAsync();

        return MapToDto(newMatch);
    }

    private async Task<PointMatch?> GetMatchAsync(string publicId)
    {
        return await _context.PointMatches
            .Include(m => m.Players)
            .FirstOrDefaultAsync(m => m.PublicId == publicId);
    }

    private static PointMatchDto MapToDto(PointMatch match)
    {
        return new PointMatchDto
        {
            PublicId = match.PublicId,
            Name = match.Name,
            HigherScoreWins = match.HigherScoreWins,
            StartingScore = match.StartingScore,
            PlayersLocked = match.PlayersLocked,
            Players = match.Players.Select(player => new PointMatchPlayerDto
            {
                Id = player.Id,
                Name = player.Name,
                Score = player.Score
            }).ToList()
        };
    }
    public async Task<PointMatchDto?> DeletePlayerAsync(string publicId, int playerId)
    {
    var match = await GetMatchAsync(publicId);

    if (match == null)
        return null;

    if (match.PlayersLocked)
        return null;

    var player = match.Players.FirstOrDefault(p => p.Id == playerId);

    if (player == null)
        return null;

    match.Players.Remove(player);
    match.UpdatedAt = DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return MapToDto(match);
    }
}