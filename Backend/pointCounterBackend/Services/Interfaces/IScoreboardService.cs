using pointCounterBackend.DTOs.Scoreboards;

namespace pointCounterBackend.Services.Interfaces;

public interface IScoreboardService
{
    Task<List<ScoreboardDto>> GetByGameIdAsync(int gameId);
    Task<ScoreboardDto?> UpdateScoreAsync(UpdateScoreboardDto dto);
}