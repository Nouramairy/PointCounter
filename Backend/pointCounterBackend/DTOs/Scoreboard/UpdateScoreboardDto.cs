using System.ComponentModel.DataAnnotations;

namespace pointCounterBackend.DTOs.Scoreboards;

public class UpdateScoreboardDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Ogiltigt spel-id.")]
    public int GameId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Ogiltigt lag-id.")]
    public int TeamId { get; set; }

    [Range(0, 1_000_000, ErrorMessage = "Poäng måste vara mellan 0 och 1 000 000.")]
    public int Score { get; set; }
}