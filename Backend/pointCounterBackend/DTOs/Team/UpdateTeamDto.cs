namespace pointCounterBackend.DTOs.Teams;

public class UpdateTeamDto
{
    public string Name { get; set; } = null!;
    public int MaximumPlayersAllowed { get; set; }

    public List<int> PlayerIds { get; set; } = new();
}