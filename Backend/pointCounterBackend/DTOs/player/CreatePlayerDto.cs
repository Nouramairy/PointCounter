namespace pointCounterBackend.DTOs.Players;

public class CreatePlayerDto
{
    public string Name { get; set; } = null!;
    public int Age { get; set; }
    public string Address { get; set; } = null!;
    public string Phone { get; set; } = null!;
}