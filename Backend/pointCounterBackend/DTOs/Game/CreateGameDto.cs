namespace pointCounterBackend.DTOs.Games;

public class CreateGameDto
{
    public string Name { get; set; } = null!;
    public int Duration { get; set; } // minutes
    public List<int> TeamIds { get; set; } = new();
}