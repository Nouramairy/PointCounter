namespace pointCounterBackend.DTOs.Scoreboards;

public class ScoreboardDto
{
    public int Id { get; set; }
    public int GameId { get; set; }
    public string GameName { get; set; } = null!;

    public int TeamId { get; set; }
    public string TeamName { get; set; } = null!;

    public int Score { get; set; }
}