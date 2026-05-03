namespace pointCounterBackend.DTOs.Players;

public class PlayerDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public int Age { get; set; }
    public string Address { get; set; } = null!;
    public string Phone { get; set; } = null!;
}