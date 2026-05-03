using Microsoft.EntityFrameworkCore;
using pointCounterBackend.Data;
using pointCounterBackend.DTOs.Games;
using pointCounterBackend.Entities;
using pointCounterBackend.Services.Interfaces;

namespace pointCounterBackend.Services;

public class GameService : IGameService
{
    private readonly AppDbContext _context;

    public GameService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<GameDto>> GetAllAsync()
    {
        return await _context.Games
            .Include(g => g.GameTeams)
                .ThenInclude(gt => gt.Team)
            .Select(g => new GameDto
            {
                Id = g.Id,
                Name = g.Name,
                Duration = g.Duration,
                Teams = g.GameTeams
                    .Select(gt => gt.Team.Name)
                    .ToList()
            })
            .ToListAsync();
    }

    public async Task<GameDto?> GetByIdAsync(int id)
    {
        return await _context.Games
            .Include(g => g.GameTeams)
                .ThenInclude(gt => gt.Team)
            .Where(g => g.Id == id)
            .Select(g => new GameDto
            {
                Id = g.Id,
                Name = g.Name,
                Duration = g.Duration,
                Teams = g.GameTeams
                    .Select(gt => gt.Team.Name)
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<GameDto?> CreateAsync(CreateGameDto dto)
    {
        if (dto.TeamIds == null || !dto.TeamIds.Any())
            return null;

        var teams = await _context.Teams
            .Where(t => dto.TeamIds.Contains(t.Id))
            .ToListAsync();

        if (teams.Count != dto.TeamIds.Count)
            return null;

        var game = new Game
        {
            Name = dto.Name,
            Duration = dto.Duration,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            GameTeams = teams.Select(team => new GameTeam
            {
                TeamId = team.Id
            }).ToList()
        };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return new GameDto
        {
            Id = game.Id,
            Name = game.Name,
            Duration = game.Duration,
            Teams = teams.Select(t => t.Name).ToList()
        };
    }

    public async Task<bool> UpdateAsync(int id, UpdateGameDto dto)
    {
        var game = await _context.Games.FindAsync(id);

        if (game == null)
            return false;

        game.Name = dto.Name;
        game.Duration = dto.Duration;
        game.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var game = await _context.Games.FindAsync(id);

        if (game == null)
            return false;

        _context.Games.Remove(game);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AddTeamToGameAsync(int gameId, int teamId)
    {
        var gameExists = await _context.Games.AnyAsync(g => g.Id == gameId);
        var teamExists = await _context.Teams.AnyAsync(t => t.Id == teamId);

        if (!gameExists || !teamExists)
            return false;

        var alreadyAdded = await _context.GameTeams
            .AnyAsync(gt => gt.GameId == gameId && gt.TeamId == teamId);

        if (alreadyAdded)
            return false;

        var gameTeam = new GameTeam
        {
            GameId = gameId,
            TeamId = teamId
        };

        _context.GameTeams.Add(gameTeam);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveTeamFromGameAsync(int gameId, int teamId)
    {
        var gameTeam = await _context.GameTeams
            .FirstOrDefaultAsync(gt => gt.GameId == gameId && gt.TeamId == teamId);

        if (gameTeam == null)
            return false;

        _context.GameTeams.Remove(gameTeam);
        await _context.SaveChangesAsync();

        return true;
    }
}