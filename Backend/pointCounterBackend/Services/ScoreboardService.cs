using Microsoft.EntityFrameworkCore;
using pointCounterBackend.Data;
using pointCounterBackend.DTOs.Scoreboards;
using pointCounterBackend.Entities;
using pointCounterBackend.Services.Interfaces;

namespace pointCounterBackend.Services;

public class ScoreboardService : IScoreboardService
{
    private readonly AppDbContext _context;

    public ScoreboardService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<ScoreboardDto>> GetByGameIdAsync(int gameId)
    {
        
        return await (
            from gt in _context.GameTeams.AsNoTracking()
            where gt.GameId == gameId
            join s in _context.Scoreboards.AsNoTracking()
                on new { gt.GameId, gt.TeamId } equals new { s.GameId, s.TeamId } into scoreJoin
            from s in scoreJoin.DefaultIfEmpty()
            orderby gt.Team.Name
            select new ScoreboardDto
            {
                Id = s != null ? s.Id : 0,
                GameId = gt.GameId,
                GameName = gt.Game.Name,
                TeamId = gt.TeamId,
                TeamName = gt.Team.Name,
                Score = s != null ? s.Score : 0
            }).ToListAsync();
    }

    public async Task<ScoreboardDto?> UpdateScoreAsync(UpdateScoreboardDto dto)
    {
        var gameExists = await _context.Games.AnyAsync(g => g.Id == dto.GameId);

        if (!gameExists)
            return null;

        var teamIsInGame = await _context.GameTeams
            .AnyAsync(gt => gt.GameId == dto.GameId && gt.TeamId == dto.TeamId);

        if (!teamIsInGame)
            return null;

        var scoreboard = await _context.Scoreboards
            .FirstOrDefaultAsync(s => s.GameId == dto.GameId && s.TeamId == dto.TeamId);

        if (scoreboard == null)
        {
            scoreboard = new Scoreboard
            {
                GameId = dto.GameId,
                TeamId = dto.TeamId,
                Score = dto.Score,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Scoreboards.Add(scoreboard);
        }
        else
        {
            scoreboard.Score = dto.Score;
            scoreboard.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return await _context.Scoreboards
            .Include(s => s.Game)
            .Include(s => s.Team)
            .Where(s => s.Id == scoreboard.Id)
            .Select(s => new ScoreboardDto
            {
                Id = s.Id,
                GameId = s.GameId,
                GameName = s.Game.Name,
                TeamId = s.TeamId,
                TeamName = s.Team.Name,
                Score = s.Score
            })
            .FirstOrDefaultAsync();
    }
}