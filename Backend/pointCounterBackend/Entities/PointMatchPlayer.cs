namespace pointCounterBackend.Entities;

public class PointMatchPlayer
{
    public int Id { get; set; }

    public int PointMatchId { get; set; }

    public string Name { get; set; } = null!;

    public int Score { get; set; }

    public int OriginalScore { get; set; }

    public PointMatch PointMatch { get; set; } = null!;
}