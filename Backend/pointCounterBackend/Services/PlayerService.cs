using Microsoft.EntityFrameworkCore;
using pointCounterBackend.Data;
using pointCounterBackend.DTOs.Players;
using pointCounterBackend.Entities;
using pointCounterBackend.Services.Interfaces;

namespace pointCounterBackend.Services;

public class PlayerService : IPlayerService
{
    private readonly AppDbContext _context;

    public PlayerService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<PlayerDto>> GetAllAsync()
    {
        return await _context.Players
            .Select(p => new PlayerDto
            {
                Id = p.Id,
                Name = p.Name,
                Age = p.Age,
                Address = p.Address,
                Phone = p.Phone
            })
            .ToListAsync();
    }

    public async Task<PlayerDto?> GetByIdAsync(int id)
    {
        return await _context.Players
            .Where(p => p.Id == id)
            .Select(p => new PlayerDto
            {
                Id = p.Id,
                Name = p.Name,
                Age = p.Age,
                Address = p.Address,
                Phone = p.Phone
            })
            .FirstOrDefaultAsync();
    }

    public async Task<PlayerDto> CreateAsync(CreatePlayerDto dto)
    {
        var player = new Player
        {
            Name = dto.Name,
            Age = dto.Age,
            Address = dto.Address,
            Phone = dto.Phone,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        return new PlayerDto
        {
            Id = player.Id,
            Name = player.Name,
            Age = player.Age,
            Address = player.Address,
            Phone = player.Phone
        };
    }

    public async Task<bool> UpdateAsync(int id, UpdatePlayerDto dto)
    {
        var player = await _context.Players.FindAsync(id);

        if (player == null)
            return false;

        player.Name = dto.Name;
        player.Age = dto.Age;
        player.Address = dto.Address;
        player.Phone = dto.Phone;
        player.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var player = await _context.Players.FindAsync(id);

        if (player == null)
            return false;

        _context.Players.Remove(player);
        await _context.SaveChangesAsync();

        return true;
    }
}