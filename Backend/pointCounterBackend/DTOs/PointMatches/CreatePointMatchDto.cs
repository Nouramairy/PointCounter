namespace pointCounterBackend.DTOs.PointMatches;

public class CreatePointMatchDto
{
    public string Name { get; set; } = null!;
    public bool HigherScoreWins { get; set; } = true;
    public int StartingScore { get; set; } = 0;
    public bool PlayersLocked { get; set; } = false;
    public List<string> PlayerNames { get; set; } = new();
}