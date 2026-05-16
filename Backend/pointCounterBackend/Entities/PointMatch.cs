namespace pointCounterBackend.Entities;

public class PointMatch
{
    public int Id { get; set; }

    public string PublicId { get; set; } = Guid.NewGuid().ToString(); // för url

    public string Name { get; set; } = null!;

    public bool HigherScoreWins { get; set; } = true;

    public int StartingScore { get; set; } = 0;

    public bool PlayersLocked { get; set; } = false;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<PointMatchPlayer> Players { get; set; } = new List<PointMatchPlayer>();
}