using System.ComponentModel.DataAnnotations;

namespace pointCounterBackend.DTOs.Games;

public class CreateGameDto : IValidatableObject
{
    [Required(ErrorMessage = "Spelnamn är obligatoriskt.")]
    [StringLength(120, MinimumLength = 1, ErrorMessage = "Spelnamnet måste vara 1–120 tecken.")]
    public string Name { get; set; } = null!;

    [Range(1, 24 * 60, ErrorMessage = "Varaktighet måste vara minst 1 minut och högst 1440 (24 h).")]
    public int Duration { get; set; }

    [MinLength(1, ErrorMessage = "Välj minst ett lag.")]
    public List<int> TeamIds { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
            yield return new ValidationResult("Spelnamnet får inte vara tomt eller bara mellanslag.", [nameof(Name)]);

        if (TeamIds.Any(id => id <= 0))
            yield return new ValidationResult("Ogiltigt lag-id.", [nameof(TeamIds)]);
    }
}