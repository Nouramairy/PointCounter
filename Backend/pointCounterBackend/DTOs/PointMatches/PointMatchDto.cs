namespace pointCounterBackend.DTOs.PointMatches;

public class PointMatchDto
{
    public string PublicId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool HigherScoreWins { get; set; }
    public int StartingScore { get; set; }
    public bool PlayersLocked { get; set; }
    public List<PointMatchPlayerDto> Players { get; set; } = new();
}