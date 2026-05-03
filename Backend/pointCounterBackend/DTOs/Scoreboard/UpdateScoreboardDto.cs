namespace pointCounterBackend.DTOs.Scoreboards;

public class UpdateScoreboardDto
{
    public int GameId { get; set; }
    public int TeamId { get; set; }
    public int Score { get; set; }
}