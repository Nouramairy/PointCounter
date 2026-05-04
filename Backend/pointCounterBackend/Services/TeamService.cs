using Microsoft.EntityFrameworkCore;
using pointCounterBackend.Data;
using pointCounterBackend.DTOs.Teams;
using pointCounterBackend.Entities;
using pointCounterBackend.Services.Interfaces;

namespace pointCounterBackend.Services;

public class TeamService : ITeamService
{
    private readonly AppDbContext _context;

    public TeamService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TeamDto>> GetAllAsync()
    {
        return await _context.Teams
            .Include(t => t.TeamPlayers)
                .ThenInclude(tp => tp.Player)
            .Select(t => new TeamDto
            {
                Id = t.Id,
                Name = t.Name,
                MaximumPlayersAllowed = t.MaximumPlayersAllowed,
                Players = t.TeamPlayers
                    .Select(tp => tp.Player.Name)
                    .ToList()
            })
            .ToListAsync();
    }

    public async Task<TeamDto?> GetByIdAsync(int id)
    {
        return await _context.Teams
            .Include(t => t.TeamPlayers)
                .ThenInclude(tp => tp.Player)
            .Where(t => t.Id == id)
            .Select(t => new TeamDto
            {
                Id = t.Id,
                Name = t.Name,
                MaximumPlayersAllowed = t.MaximumPlayersAllowed,
                Players = t.TeamPlayers
                    .Select(tp => tp.Player.Name)
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<TeamDto?> CreateAsync(CreateTeamDto dto)
    {
        if (dto.PlayerIds == null || !dto.PlayerIds.Any())
            return null;

        if (dto.PlayerIds.Count > dto.MaximumPlayersAllowed)
            return null;

        var players = await _context.Players
            .Where(p => dto.PlayerIds.Contains(p.Id))
            .ToListAsync();

        if (players.Count != dto.PlayerIds.Count)
            return null;

        var team = new Team
        {
            Name = dto.Name,
            MaximumPlayersAllowed = dto.MaximumPlayersAllowed,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            TeamPlayers = players.Select(player => new TeamPlayer
            {
                PlayerId = player.Id
            }).ToList()
        };

        _context.Teams.Add(team);
        await _context.SaveChangesAsync();

        return new TeamDto
        {
            Id = team.Id,
            Name = team.Name,
            MaximumPlayersAllowed = team.MaximumPlayersAllowed,
            Players = players.Select(p => p.Name).ToList()
        };
    }

    public async Task<bool> UpdateAsync(int id, UpdateTeamDto dto)
    {
      //  var team = await _context.Teams.FindAsync(id);

        var team = await _context.Teams
        .Include(t => t.TeamPlayers)
        .FirstOrDefaultAsync(t => t.Id == id);

        if (team == null)
            return false;

        team.Name = dto.Name;
        team.MaximumPlayersAllowed = dto.MaximumPlayersAllowed;
        await _context.SaveChangesAsync();

        var updatedTeam = await _context.Teams.FindAsync(id);

        var teamplayers = dto.PlayerIds.Select(playerId => new TeamPlayer
        {
            TeamId = id,
            PlayerId = playerId
        }).ToList();
        if(updatedTeam.MaximumPlayersAllowed >= teamplayers.Count)
        {
            team.TeamPlayers = teamplayers;
            team.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        else
        {
            return false;
        }
        
        // here TeamPlayers is an object collection of TeamPlayer Entity
        // but PlayerIds is a list of intergers, they dont mapp or equal. 
        // we have to build a team player with the ids we have then assign it to team.TeamPlayers
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var team = await _context.Teams.FindAsync(id);

        if (team == null)
            return false;

        _context.Teams.Remove(team);
        await _context.SaveChangesAsync();

        return true;
    }
}