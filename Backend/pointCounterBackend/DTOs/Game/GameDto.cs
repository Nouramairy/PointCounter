namespace pointCounterBackend.DTOs.Games;

public class GameDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Duration { get; set; }

    public List<string> Teams { get; set; } = new();
}