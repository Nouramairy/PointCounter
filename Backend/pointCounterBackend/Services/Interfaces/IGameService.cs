using pointCounterBackend.DTOs.Games;

namespace pointCounterBackend.Services.Interfaces;

public interface IGameService
{
    Task<List<GameDto>> GetAllAsync();
    Task<GameDto?> GetByIdAsync(int id);
    Task<GameDto?> CreateAsync(CreateGameDto dto);
    Task<bool> UpdateAsync(int id, UpdateGameDto dto);
    Task<bool> DeleteAsync(int id);

    Task<bool> AddTeamToGameAsync(int gameId, int teamId);
    Task<bool> RemoveTeamFromGameAsync(int gameId, int teamId);
}