namespace pointCounterBackend.DTOs.Games;

public class UpdateGameDto
{
    public string Name { get; set; } = null!;
    public int Duration { get; set; }
}