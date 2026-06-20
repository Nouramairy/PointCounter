using pointCounterBackend.DTOs.PointMatches;

namespace pointCounterBackend.Services.Interfaces;

public interface IPointMatchService
{
    Task<PointMatchDto> CreateAsync(CreatePointMatchDto dto);

    Task<PointMatchDto?> GetByPublicIdAsync(string publicId);

    Task<PointMatchDto?> UpdateScoreAsync(
        string publicId,
        int playerId,
        UpdatePointScoreDto dto);

    Task<PointMatchDto?> AddPlayerAsync(
        string publicId,
        AddPointMatchPlayerDto dto);

    Task<PointMatchDto?> UpdatePlayerNameAsync(
        string publicId,
        int playerId,
        UpdatePointMatchPlayerNameDto dto);

    Task<PointMatchDto?> DeletePlayerAsync(string publicId, int playerId);  
    Task<PointMatchDto?> RestartAsync(string publicId);

    Task<PointMatchDto?> CloneAsync(string publicId);
}