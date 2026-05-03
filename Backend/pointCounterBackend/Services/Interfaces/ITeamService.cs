using pointCounterBackend.DTOs.Teams;

namespace pointCounterBackend.Services.Interfaces;

public interface ITeamService
{
    Task<List<TeamDto>> GetAllAsync();
    Task<TeamDto?> GetByIdAsync(int id);
    Task<TeamDto?> CreateAsync(CreateTeamDto dto);
    Task<bool> UpdateAsync(int id, UpdateTeamDto dto);
    Task<bool> DeleteAsync(int id);
}