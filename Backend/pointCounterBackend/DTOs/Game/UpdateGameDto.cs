using System.ComponentModel.DataAnnotations;

namespace pointCounterBackend.DTOs.Games;

public class UpdateGameDto : IValidatableObject
{
    [Required(ErrorMessage = "Spelnamn är obligatoriskt.")]
    [StringLength(120, MinimumLength = 1, ErrorMessage = "Spelnamnet måste vara 1–120 tecken.")]
    public string Name { get; set; } = null!;

    [Range(1, 24 * 60, ErrorMessage = "Varaktighet måste vara minst 1 minut och högst 1440 (24 h).")]
    public int Duration { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Name))
            yield return new ValidationResult("Spelnamnet får inte vara tomt eller bara mellanslag.", [nameof(Name)]);
    }
}