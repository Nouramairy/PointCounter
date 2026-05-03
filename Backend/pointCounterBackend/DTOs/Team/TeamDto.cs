namespace pointCounterBackend.DTOs.Teams;

public class TeamDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int MaximumPlayersAllowed { get; set; }
    public List<string> Players { get; set; } = new();
}